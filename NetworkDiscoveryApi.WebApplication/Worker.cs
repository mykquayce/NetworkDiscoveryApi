using Dawn;
using Helpers.Networking.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.WebApplication;

public class Worker : BackgroundService
{
	private readonly IDictionary<PhysicalAddress, ICollection<string>> _aliasesLookup;
	private readonly ILogger<Worker> _logger;
	private readonly Services.IMemoryCacheService<DhcpLease> _memoryCacheService;
	private readonly Services.IRouterService _routerService;

	public Worker(
		ILogger<Worker> logger,
		Services.IMemoryCacheService<DhcpLease> memoryCacheService,
		Services.IRouterService routerService,
		IOptions<IReadOnlyDictionary<string, PhysicalAddress>> aliasesOptions)
	{
		_aliasesLookup = Guard.Argument(aliasesOptions).NotNull().Wrap(o => o.Value)
			.NotNull().NotEmpty().Value.Invert();
		_logger = Guard.Argument(logger).NotNull().Value;
		_memoryCacheService = Guard.Argument(memoryCacheService).NotNull().Value;
		_routerService = Guard.Argument(routerService).NotNull().Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			(_memoryCacheService as MemoryCache)?.Compact(1d);

			_logger.LogInformation("{now}: fetching", DateTime.UtcNow.ToString("O"));

			var leases = _routerService.GetLeasesAsync();
			var soonest = await CacheLeasesAsync(leases);

			_logger.LogInformation("{now}: waiting till {next}", DateTime.UtcNow.ToString("O"), soonest.ToString("O"));

			await Task.Delay((int)(soonest - DateTime.UtcNow).TotalMilliseconds, stoppingToken);
		}
	}

	private async Task<DateTime> CacheLeasesAsync(IAsyncEnumerable<DhcpLease> leases)
	{
		var soonest = DateTime.MaxValue;

		await foreach (var lease in leases)
		{
			CacheLease(lease);
			if (lease.Expiration < soonest) soonest = lease.Expiration;
		}

		return soonest;
	}

	private void CacheLease(DhcpLease lease)
	{
		var ok = _aliasesLookup.TryGetValue(lease.PhysicalAddress, out var aliases);

		if (ok)
		{
			foreach (var alias in aliases!)
			{
				cache(alias);
			}
		}

		cache(lease.PhysicalAddress);
		cache(lease.IPAddress);
		if (!string.IsNullOrWhiteSpace(lease.HostName)) cache(lease.HostName);

		void cache(object key) => _memoryCacheService.Set(key, lease, lease.Expiration);
	}
}
