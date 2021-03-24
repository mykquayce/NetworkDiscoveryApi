using Microsoft.Extensions.Configuration;
using System;

namespace NetworkDiscoveryApi.Services.Tests.Fixtures
{
	public sealed class RouterServiceFixture : IDisposable
	{
		public RouterServiceFixture()
		{
			var configuration = new ConfigurationBuilder()
				.AddUserSecrets(this.GetType().Assembly)
				.Build();

			var config = configuration.GetSection("Router")
				.Get<Helpers.SSH.Services.Concrete.SSHService.Config>();

			var sshService = new Helpers.SSH.Services.Concrete.SSHService(config);

			RouterService = new Concrete.RouterService(sshService);
		}

		public IRouterService RouterService { get; }

		#region dispose
		public void Dispose() => RouterService.Dispose();
		#endregion dispose
	}
}
