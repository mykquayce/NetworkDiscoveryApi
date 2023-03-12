using Dawn;
using Helpers.Networking.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.WebApplication;

public class Worker : BackgroundService, ICustomWorkerStarter
{
	private readonly IDictionary<PhysicalAddress, ICollection<string>> _aliasesLookup;
	private readonly ILogger<Worker> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly IEnumerableMemoryCache _memoryCache;

	public Worker(
		ILogger<Worker> logger,
		IEnumerableMemoryCache memoryCache,
		IOptions<Models.AliasesLookup> aliasesOptions,
		IServiceProvider serviceProvider)
	{
		IEnumerable<KeyValuePair<string, PhysicalAddress>> kvps = Guard.Argument(aliasesOptions).NotNull().Wrap(o => o.Value)
			.NotNull().NotEmpty().Value;
		_aliasesLookup = kvps.Invert();
		_logger = Guard.Argument(logger).NotNull().Value;
		_memoryCache = Guard.Argument(memoryCache).NotNull().Value;
		_serviceProvider = Guard.Argument(serviceProvider).NotNull().Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			var now = DateTime.UtcNow;
			_memoryCache.Clear();

			_logger.LogInformation("{now:O}: fetching", now);

			var expirations = new List<TimeSpan> { TimeSpan.FromHours(1), };
			var leases = GetLeasesAsync();

			await foreach (var lease in leases)
			{
				CacheLease(lease);
				expirations.Add(lease.Expiration - now);
			}

			var soonest = expirations.Min().Clamp(TimeSpan.FromMinutes(5), TimeSpan.FromHours(1));
			_logger.LogInformation("{now:O}: sleeping for {soonest:F0}min(s)", now, Math.Round(soonest.TotalMinutes));
			await Task.Delay(millisecondsDelay: (int)soonest.TotalMilliseconds, stoppingToken);
		}
	}

	private IAsyncEnumerable<DhcpLease> GetLeasesAsync()
	{
		var service = _serviceProvider.GetRequiredService<Services.IRouterService>();
		return service.GetLeasesAsync();
	}

	private void CacheLease(DhcpLease lease)
	{
		var (expiration, mac, ip, host, _) = lease;

		// check for aliases
		var ok = _aliasesLookup.TryGetValue(lease.PhysicalAddress, out var aliases);

		// log
		{
			var values = new object?[] { host?.ToLowerInvariant(), ip, mac, }.Union(aliases ?? Array.Empty<string>());
			_logger.LogInformation("caching ({values})", string.Join(',', values));
		}

		// cache the aliases
		if (ok)
		{
			foreach (var alias in aliases!)
			{
				cache(alias);
			}
		}

		// cache the mac, ip, and hostname
		cache(mac);
		cache(ip);
		if (!string.IsNullOrWhiteSpace(host)) cache(host.ToLowerInvariant());

		void cache(object key) => _memoryCache.Set(key, lease, expiration);
	}
}
