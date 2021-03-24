using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NetworkDiscoveryApi.Services.Tests
{
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
			IList<Models.DhcpEntry> before = await _routerService.GetDhcpLeasesAsync().ToListAsync();

			using var cache = new Services.Concrete.CachingService<IList<Models.DhcpEntry>>();

			Assert.NotNull(before);
			Assert.NotEmpty(before);

			cache.Set(before);

			var ok = cache.TryGet(out var after);

			Assert.True(ok);
			Assert.NotNull(after);
			Assert.Same(before, after);
		}
	}
}
