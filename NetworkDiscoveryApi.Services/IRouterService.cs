namespace NetworkDiscoveryApi.Services;

public interface IRouterService
{
	IAsyncEnumerable<Models.DhcpLease> GetLeasesAsync();
}
