using Dawn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.WebApplication.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RouterController : ControllerBase
{
	private readonly ILogger<RouterController> _logger;
	private readonly IEnumerableMemoryCache _memoryCache;
	private readonly ICustomWorkerStarter _hostedService;

	public RouterController(
		ILogger<RouterController> logger,
		IEnumerableMemoryCache memoryCache,
		ICustomWorkerStarter hostedService)
	{
		_memoryCache = Guard.Argument(memoryCache).NotNull().Value;
		_hostedService = Guard.Argument(hostedService).NotNull().Value;
		_logger = Guard.Argument(logger).NotNull().Value;
	}

	[HttpPut]
	[Route("reset")]
	public async Task<IActionResult> Reset()
	{
		_logger.LogInformation("starting hosted service");
		await _hostedService.StartAsync();
		_logger.LogInformation("started hosted service");
		return Ok();
	}

	[HttpGet]
	[Route("{keyString:minlength(1)}")]
	public IActionResult Get(string keyString)
	{
		object key;
		{
			if (PhysicalAddress.TryParse(keyString, out var mac))
			{
				key = mac;
			}
			else if (IPAddress.TryParse(keyString, out var ip))
			{
				key = ip;
			}
			else
			{
				key = keyString.ToLowerInvariant();
			}
		}

		var ok = _memoryCache.TryGetValue<Helpers.Networking.Models.DhcpLease>(key, out var lease);

		if (ok)
		{
			var (expiration, physicalAddress, ipAddress, hostName, identifier) = lease!;
			_logger.LogInformation("{key} resolved to {physicalAddress}, {ipAddress}", key, physicalAddress, ipAddress);
			return Ok(new
			{
				expiration = expiration.ToString("O"),
				ipAddress = ipAddress.ToString(),
				physicalAddress = physicalAddress.ToString().ToLowerInvariant(),
				hostName,
				identifier,
			});
		}

		return NotFound(new { key = keyString, });
	}

	[HttpGet]
	public IActionResult Get()
	{
		var leases = _memoryCache.Values.Distinct().ToArray();

		if (leases.Any())
		{
			return Ok(leases);
		}

		return NoContent();
	}
}
