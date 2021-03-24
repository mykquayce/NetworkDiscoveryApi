using Dawn;
using Microsoft.AspNetCore.Mvc;
using NetworkDiscoveryApi.Services;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkDiscoveryApi.WebApplication.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
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
				return Ok(entries.Select(e => (Models.DhcpEntry)e));
			}

			return NotFound();
		}
	}
}
