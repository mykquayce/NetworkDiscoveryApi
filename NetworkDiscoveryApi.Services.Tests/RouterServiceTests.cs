using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NetworkDiscoveryApi.Services.Tests
{
	public sealed class RouterServiceTests : IDisposable
	{
		private readonly IRouterService _sut;

		public RouterServiceTests()
		{
			var configuration = new ConfigurationBuilder()
				.AddUserSecrets(this.GetType().Assembly)
				.Build();

			var config = configuration.GetSection("Router")
				.Get<Helpers.SSH.Services.Concrete.SSHService.Config>();

			var sshService = new Helpers.SSH.Services.Concrete.SSHService(config);

			_sut = new Concrete.RouterService(sshService);
		}

		public void Dispose() => _sut.Dispose();

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
