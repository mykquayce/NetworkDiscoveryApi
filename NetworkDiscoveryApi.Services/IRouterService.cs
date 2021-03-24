using System;
using System.Collections.Generic;

namespace NetworkDiscoveryApi.Services
{
	public interface IRouterService : IDisposable
	{
		IAsyncEnumerable<Helpers.Networking.Models.DhcpEntry> GetDhcpLeasesAsync();
	}
}
