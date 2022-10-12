using Dawn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetworkDiscoveryApi.Services;
using NetworkDiscoveryApi.Services.Models;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.WebApplication.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RouterController : ControllerBase
{
	private readonly IMemoryCacheService<DhcpLease> _memoryCacheService;

	public RouterController(IMemoryCacheService<DhcpLease> memoryCacheService)
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
			var (expiry, mac, ip, hostname, alias) = _memoryCacheService.Get(key);
			return Ok(new
			{
				ip = ip.ToString(),
				mac = mac.ToString().ToLowerInvariant(),
				hostname,
				alias,
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
