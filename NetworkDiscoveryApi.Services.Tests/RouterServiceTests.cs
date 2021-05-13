using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NetworkDiscoveryApi.Services.Tests
{
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
			var entries = await _sut.GetDhcpLeasesAsync().ToListAsync();

			Assert.NotNull(entries);
			Assert.NotEmpty(entries);

			foreach (var entry in entries)
			{
				Assert.NotNull(entry);
				Assert.NotNull(entry.PhysicalAddress);
				Assert.NotNull(entry.IPAddress);
			}
		}

		[Theory]
		[InlineData(10)]
		public async Task CacheTests(int count)
		{
			var times = new List<long>(capacity: count);

			while (count-- > 0)
			{
				var stopwatch = Stopwatch.StartNew();
				await _sut.GetDhcpLeasesAsync().ToListAsync();
				stopwatch.Start();
				times.Add(stopwatch.ElapsedTicks);
			}

			// Assert, first is much slower than any of the rest
			Assert.NotEmpty(times);
			Assert.True((times[0] / 2d) > times.Skip(1).Max());
		}
	}
}
