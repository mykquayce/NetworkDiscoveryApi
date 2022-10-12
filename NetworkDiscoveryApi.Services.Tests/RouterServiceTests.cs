using Xunit;

namespace NetworkDiscoveryApi.Services.Tests;

public sealed class RouterServiceTests : IClassFixture<Fixtures.RouterServiceFixture>
{
	private readonly IRouterService _sut;

	public RouterServiceTests(Fixtures.RouterServiceFixture fixture)
	{
		_sut = fixture.RouterService;
	}

	[Fact]
	public async Task GetDhcpLeases()
	{
		var entries = await _sut.GetLeasesAsync().ToListAsync();

		Assert.NotNull(entries);
		Assert.NotEmpty(entries);
		Assert.DoesNotContain(default, entries);
	}
}
