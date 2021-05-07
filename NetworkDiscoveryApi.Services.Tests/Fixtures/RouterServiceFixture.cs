using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.Services.Tests.Fixtures
{
	public sealed class RouterServiceFixture : IDisposable
	{
		public RouterServiceFixture()
		{
			var sshServiceMock = new Mock<Helpers.SSH.Services.ISSHService>();

			var entries = new Helpers.Networking.Models.DhcpEntry[1]
			{
				new(DateTime.MaxValue, PhysicalAddress.None, IPAddress.None, "localhost", "home"),
			};

			sshServiceMock
				.Setup(s => s.GetDhcpLeasesAsync())
				.Returns(entries.ToAsyncEnumerable());

			var cachingService = new Services.Concrete.CachingService<IList<Models.DhcpEntry>>();

			RouterService = new Concrete.RouterService(sshServiceMock.Object, cachingService);
		}

		public IRouterService RouterService { get; }

		#region dispose
		public void Dispose() => RouterService.Dispose();
		#endregion dispose
	}
}
