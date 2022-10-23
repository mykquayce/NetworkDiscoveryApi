using Dawn;
using Helpers.Networking.Models;
using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.WebApplication;

public class Worker : BackgroundService, ICustomWorkerStarter
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
			_memoryCacheService.Clear();

			_logger.LogInformation("{now:O}: fetching", DateTime.UtcNow);

			var soonest = DateTime.MaxValue;
			var leases = _routerService.GetLeasesAsync();

			await foreach (var lease in leases)
			{
				CacheLease(lease);
				if (lease.Expiration < DateTime.MaxValue) { soonest = lease.Expiration; }
			}

			_logger.LogInformation("{now:O}: waiting till {next:O}", DateTime.UtcNow, soonest);

			await Task.Delay((int)(soonest - DateTime.UtcNow).TotalMilliseconds, stoppingToken);
		}
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
