namespace NetworkDiscoveryApi.WebApplication;

public interface ICustomWorkerStarter
{
	Task StartAsync(CancellationToken cancellationToken = default);
}
