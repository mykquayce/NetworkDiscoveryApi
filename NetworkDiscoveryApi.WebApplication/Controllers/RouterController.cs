using Dawn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetworkDiscoveryApi.Services;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.WebApplication.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RouterController : ControllerBase
{
	private readonly ILogger<RouterController> _logger;
	private readonly IMemoryCacheService<Helpers.Networking.Models.DhcpLease> _memoryCacheService;
	private readonly ICustomWorkerStarter _hostedService;

	public RouterController(
		ILogger<RouterController> logger,
		IMemoryCacheService<Helpers.Networking.Models.DhcpLease> memoryCacheService,
		ICustomWorkerStarter hostedService)
	{
		_memoryCacheService = Guard.Argument(memoryCacheService).NotNull().Value;
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

		try
		{
			var (expiration, physicalAddress, ipAddress, hostName, identifier) = _memoryCacheService.Get(key);
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
		catch (ArgumentOutOfRangeException)
		{
			return NotFound(new { key = keyString, });
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new { key = keyString, ex.Message, });
		}
	}
}
