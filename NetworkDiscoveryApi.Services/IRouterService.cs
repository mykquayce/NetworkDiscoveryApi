using System;
using System.Collections.Generic;

namespace NetworkDiscoveryApi.Services
{
	public interface IRouterService : IDisposable
	{
		IAsyncEnumerable<Models.DhcpEntry> GetDhcpLeasesAsync();
	}
}
