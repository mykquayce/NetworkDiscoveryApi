namespace NetworkDiscoveryApi.Services.Tests.Fixtures;

public sealed class SSHServiceFixture : IDisposable
{
	public SSHServiceFixture()
	{
		var config = new Helpers.SSH.Config("192.168.1.10", 22, "root", "5d2d7995d1b846695bba6d54291557bf56d084dab71ddb7df3777e6259dc6655");
		Client = new Helpers.SSH.Concrete.Client(config);
		Service = new Helpers.SSH.Concrete.Service(Client);
	}

	public Helpers.SSH.IClient Client { get; }
	public Helpers.SSH.IService Service { get; }

	public void Dispose() => Client.Dispose();
}
