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
				var query = from e in entries
							select new
							{
								expiration = e.Expiration.ToString("O"),
								ip = e.IPAddress.ToString(),
								e.HostName,
								mac = e.PhysicalAddress.ToString().ToLowerInvariant(),
							};

				return Ok(query);
			}

			return NotFound();
		}
	}
}
