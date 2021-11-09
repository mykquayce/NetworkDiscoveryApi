using Dawn;
using Helpers.SSH;

namespace NetworkDiscoveryApi.Services.Concrete;

public class RouterService : IRouterService
{
	private readonly IService _sshService;
	private readonly ICachingService<IList<Helpers.Networking.Models.DhcpLease>> _cachingService;

	public RouterService(IService sshService, ICachingService<IList<Helpers.Networking.Models.DhcpLease>> cachingService)
	{
		_sshService = Guard.Argument(() => sshService).NotNull().Value;
		_cachingService = Guard.Argument(() => cachingService).NotNull().Value;
	}

	public async IAsyncEnumerable<Helpers.Networking.Models.DhcpLease> GetDhcpLeasesAsync()
	{
		if (!_cachingService.TryGet(out var entries))
		{
			entries = await _sshService.GetDhcpLeasesAsync().ToListAsync();

			_cachingService.Set(entries);
		}

		foreach (var entry in entries!)
		{
			yield return entry;
		}
	}
}
