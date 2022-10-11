using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.Services.Tests.Fixtures;

public sealed class MemoryCacheFixture : IDisposable
{
	public MemoryCacheFixture()
	{
		MemoryCache = new MemoryCache(new MemoryCacheOptions());

		var lease = new Helpers.Networking.Models.DhcpLease(
			DateTime.UnixEpoch.AddSeconds(1_665_487_302),
			PhysicalAddress.Parse("f02f74d209a5"),
			IPAddress.Parse("192.168.1.229"),
			"malik10",
			"01:f0:2f:74:d2:09:a5");

		MemoryCache.Set(lease.HostName!, lease, lease.Expiration);
		MemoryCache.Set(lease.IPAddress, lease, lease.Expiration);
		MemoryCache.Set(lease.PhysicalAddress, lease, lease.Expiration);
	}

	public IMemoryCache MemoryCache { get; }

	public void Dispose() => MemoryCache.Dispose();
}
