using System;
using System.Collections.Generic;

namespace NetworkDiscoveryApi.Services
{
	public interface IRouterService
	{
		IAsyncEnumerable<Helpers.Networking.Models.DhcpLease> GetDhcpLeasesAsync();
	}
}
