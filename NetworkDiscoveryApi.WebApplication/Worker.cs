using Dawn;
using Microsoft.Extensions.Caching.Memory;

namespace NetworkDiscoveryApi.WebApplication;

public class Worker : BackgroundService
{
	private readonly ILogger<Worker> _logger;
	private readonly IMemoryCache _memoryCache;
	private readonly Helpers.SSH.IService _service;

	public Worker(ILogger<Worker> logger, IMemoryCache memoryCache, Helpers.SSH.IService service)
	{
		_logger = Guard.Argument(logger).NotNull().Value;
		_memoryCache = Guard.Argument(memoryCache).NotNull().Value;
		_service = Guard.Argument(service).NotNull().Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			(_memoryCache as MemoryCache)?.Compact(1d);

			_logger.LogInformation("{now} fetching", DateTime.UtcNow.ToString("O"));
			var soonest = DateTime.MaxValue;
			var leases = _service.GetDhcpLeasesAsync();

			await foreach (var lease in leases)
			{
				var (expiry, mac, ip, hostname, _) = lease;

				foreach (var key in new object?[3] { mac, ip, hostname, })
				{
					if (key is not null)
					{
						_memoryCache.Set(key, lease, absoluteExpiration: expiry);
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
