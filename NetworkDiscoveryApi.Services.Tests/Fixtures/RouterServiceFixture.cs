using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.Services.Tests.Fixtures;

public sealed class RouterServiceFixture : IDisposable
{
	private readonly SSHServiceFixture _sshServiceFixture = new();

	public RouterServiceFixture()
	{
		var aliases = new Dictionary<string, PhysicalAddress>
		{
			["vr rear"] = PhysicalAddress.Parse("003192e1a474"),
		};

		RouterService = new Concrete.RouterService(_sshServiceFixture.Service, Options.Create(aliases));
	}

	public IRouterService RouterService { get; }

	public void Dispose() => _sshServiceFixture.Dispose();
}
