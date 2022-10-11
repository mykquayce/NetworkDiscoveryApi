using System.Net.NetworkInformation;
using Xunit;

namespace NetworkDiscoveryApi.Services.Tests;

public sealed class RouterServiceTests : IClassFixture<Fixtures.RouterServiceFixture>
{
	private readonly IRouterService _sut;

	public RouterServiceTests(Fixtures.RouterServiceFixture fixture)
	{
		_sut = fixture.RouterService;
	}

	[Theory]
	[InlineData("f02f74d209a5")]
	public void GetDhcpLeases(string macString)
	{
		var mac = PhysicalAddress.Parse(macString);
		var entry = _sut.GetLeaseByPhysicalAddress(mac);

		Assert.NotNull(entry);
	}
}
