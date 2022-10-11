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
	private readonly IRouterService _routerService;

	public RouterController(IRouterService routerService)
	{
		_routerService = Guard.Argument(() => routerService).NotNull().Value;
	}

	[HttpGet]
	[Route("{mac:length(12,17)}")]
	public IActionResult Get(string mac)
	{
		if (!PhysicalAddress.TryParse(mac, out var physicalAddress))
		{
			return BadRequest(new { message = "unable to parse mac", mac, });
		}

		try
		{
			var entry = _routerService.GetLeaseByPhysicalAddress(physicalAddress);
			return Ok(entry);
		}
		catch (ArgumentOutOfRangeException)
		{
			return NotFound(new { MAC = physicalAddress.ToString().ToLowerInvariant(), });
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new { MAC = physicalAddress.ToString().ToLowerInvariant(), ex.Message, });
		}
	}
}
