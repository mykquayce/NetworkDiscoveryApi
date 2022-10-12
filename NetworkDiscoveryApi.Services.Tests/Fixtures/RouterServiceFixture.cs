namespace NetworkDiscoveryApi.Services.Tests.Fixtures;

public sealed class RouterServiceFixture : IDisposable
{
	private readonly SSHServiceFixture _sshServiceFixture = new();

	public RouterServiceFixture()
	{
		RouterService = new Concrete.RouterService(_sshServiceFixture.Service);
	}

	public IRouterService RouterService { get; }

	public void Dispose() => _sshServiceFixture.Dispose();
}
