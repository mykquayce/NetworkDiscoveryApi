using Dawn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetworkDiscoveryApi.Services;
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
	public async Task<IActionResult> Get()
	{
		var entries = await _routerService.GetDhcpLeasesAsync().ToListAsync();

		if (entries.Any())
		{
			return Ok(entries);
		}

		return NotFound();
	}

	[HttpGet]
	[Route("{mac:length(12,17)}")]
	public async Task<IActionResult> Get(string mac)
	{
		if (!PhysicalAddress.TryParse(mac, out var physicalAddress))
		{
			return BadRequest(new { message = "unable to parse mac", mac, });
		}

		var entries = _routerService.GetDhcpLeasesAsync();

		await foreach (var entry in entries)
		{
			if (entry.PhysicalAddress.Equals(physicalAddress))
			{
				return Ok(entry);
			}
		}

		return NotFound();
	}
}
