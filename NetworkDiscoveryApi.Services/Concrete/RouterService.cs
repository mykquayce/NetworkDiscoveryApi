using Dawn;

namespace NetworkDiscoveryApi.Services.Concrete;

public class RouterService : IRouterService
{
	private readonly Helpers.SSH.IService _sshService;

	public RouterService(
		Helpers.SSH.IService sshService)
	{
		_sshService = Guard.Argument(sshService).NotNull().Value;
	}

	public IAsyncEnumerable<Helpers.Networking.Models.DhcpLease> GetLeasesAsync()
	{
		return _sshService.GetDhcpLeasesAsync();
	}
}
