using Xunit;

namespace NetworkDiscoveryApi.Services.Tests;

public class CachingServiceTests : IClassFixture<Fixtures.RouterServiceFixture>
{
	private readonly IRouterService _routerService;

	public CachingServiceTests(Fixtures.RouterServiceFixture fixture)
	{
		_routerService = fixture.RouterService;
	}

	[Fact]
	public async Task Cache()
	{
		IList<Helpers.Networking.Models.DhcpLease> before = await _routerService.GetDhcpLeasesAsync().ToListAsync();

		using var cache = new Services.Concrete.CachingService<IList<Helpers.Networking.Models.DhcpLease>>();

		Assert.NotNull(before);
		Assert.NotEmpty(before);

		cache.Set(before);

		var ok = cache.TryGet(out var after);

		Assert.True(ok);
		Assert.NotNull(after);
		Assert.Same(before, after);
	}
}
