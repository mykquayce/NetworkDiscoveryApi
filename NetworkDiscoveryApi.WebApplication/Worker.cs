using Dawn;
using Microsoft.Extensions.Caching.Memory;

namespace NetworkDiscoveryApi.WebApplication;

public class Worker : BackgroundService
{
	private readonly ILogger<Worker> _logger;
	private readonly Services.IMemoryCacheService<Services.Models.DhcpLease> _memoryCacheService;
	private readonly Services.IRouterService _routerService;

	public Worker(
		ILogger<Worker> logger,
		Services.IMemoryCacheService<Services.Models.DhcpLease> memoryCacheService,
		Services.IRouterService routerService)
	{
		_logger = Guard.Argument(logger).NotNull().Value;
		_memoryCacheService = Guard.Argument(memoryCacheService).NotNull().Value;
		_routerService = Guard.Argument(routerService).NotNull().Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			(_memoryCacheService as MemoryCache)?.Compact(1d);

			_logger.LogInformation("{now} fetching", DateTime.UtcNow.ToString("O"));
			var soonest = DateTime.MaxValue;
			var leases = _routerService.GetLeasesAsync();

			await foreach (var lease in leases)
			{
				var (expiry, mac, ip, hostname, alias) = lease;

				foreach (var key in new object?[4] { alias, hostname, ip, mac, })
				{
					if (key is not null)
					{
						_memoryCacheService.Set(key, lease, expiration: expiry);
					}
				}

				if (expiry < soonest)
				{
					soonest = expiry;
				}
			}

			_logger.LogInformation("{now} waiting till {next}", DateTime.UtcNow.ToString("O"), soonest.ToString("O"));

			await Task.Delay((int)(soonest - DateTime.UtcNow).TotalMilliseconds, stoppingToken);
		}
	}
}
