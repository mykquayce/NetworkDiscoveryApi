using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.Services.Tests.Fixtures
{
	public class RouterServiceFixture
	{
		public RouterServiceFixture()
		{
			var sshServiceMock = new Mock<Helpers.SSH.IService>();

			var entries = new Helpers.Networking.Models.DhcpLease[1]
			{
				new(DateTime.MaxValue, PhysicalAddress.None, IPAddress.None, "localhost", "home"),
			};

			sshServiceMock
				.Setup(s => s.GetDhcpLeasesAsync())
				.Returns(entries.ToAsyncEnumerable());

			var cachingService = new Services.Concrete.CachingService<IList<Helpers.Networking.Models.DhcpLease>>();

			RouterService = new Concrete.RouterService(sshServiceMock.Object, cachingService);
		}

		public IRouterService RouterService { get; }
	}
}
