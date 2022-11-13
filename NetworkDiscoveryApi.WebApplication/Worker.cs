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
		IOptions<IReadOnlyDictionary<string, PhysicalAddress>> aliasesOptions,
		IServiceProvider serviceProvider)
	{
		_aliasesLookup = Guard.Argument(aliasesOptions).NotNull().Wrap(o => o.Value)
			.NotNull().NotEmpty().Value.Invert();
		_logger = Guard.Argument(logger).NotNull().Value;
		_memoryCache = Guard.Argument(memoryCache).NotNull().Value;
		_serviceProvider = Guard.Argument(serviceProvider).NotNull().Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			_memoryCache.Clear();

			_logger.LogInformation("{now:O}: fetching", DateTime.UtcNow);

			var soonest = DateTime.MaxValue;
			var leases = GetLeasesAsync();

			await foreach (var lease in leases)
			{
				CacheLease(lease);
				if (lease.Expiration < DateTime.MaxValue) { soonest = lease.Expiration; }
			}

			_logger.LogInformation("{now:O}: waiting till {next:O}", DateTime.UtcNow, soonest);

			await Task.Delay((int)(soonest - DateTime.UtcNow).TotalMilliseconds, stoppingToken);
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
