using Helpers.Networking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
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
			var before = await _routerService.GetDhcpLeasesAsync().ToListAsync();

			Assert.NotNull(before);
			Assert.NotEmpty(before);

			var cache = MemoryCache.Default;

			var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1), };

			cache.Set("DhcpEntries", before, policy);

			var after = cache["DhcpEntries"] as IReadOnlyCollection<DhcpEntry>;

			Assert.NotNull(after);
			Assert.NotEmpty(after);
		}
	}
}
