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
	private readonly IMemoryCacheService<Helpers.Networking.Models.DhcpLease> _memoryCacheService;

	public RouterController(IMemoryCacheService<Helpers.Networking.Models.DhcpLease> memoryCacheService)
	{
		_memoryCacheService = Guard.Argument(memoryCacheService).NotNull().Value;
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
