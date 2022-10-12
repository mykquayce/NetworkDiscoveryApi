namespace NetworkDiscoveryApi.Services;

public interface IRouterService
{
	IAsyncEnumerable<Helpers.Networking.Models.DhcpLease> GetLeasesAsync();
}
