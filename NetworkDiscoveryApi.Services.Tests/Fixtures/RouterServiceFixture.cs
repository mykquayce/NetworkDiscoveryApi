using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

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
			var cachingService = new Services.Concrete.CachingService<IList<Models.DhcpEntry>>();

			RouterService = new Concrete.RouterService(sshService, cachingService);
		}

		public IRouterService RouterService { get; }

		#region dispose
		public void Dispose() => RouterService.Dispose();
		#endregion dispose
	}
}
