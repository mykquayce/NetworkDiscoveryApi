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
	}
}
