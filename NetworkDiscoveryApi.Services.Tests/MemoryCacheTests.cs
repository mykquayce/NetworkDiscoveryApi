using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.NetworkInformation;
using Xunit;

namespace NetworkDiscoveryApi.Services.Tests;

public class MemoryCacheTests
{
	[Theory]
	[InlineData("ecb5fa18e324", "hello world")]
	public void PhysicalAddressAsKeyTests(string keyString, string value)
	{
		bool ok;
		string? actual;
		{
			using IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
			{
				var key = PhysicalAddress.Parse(keyString);
				memoryCache.Set(key, value);
			}
			{
				var key = PhysicalAddress.Parse(keyString);
				ok = memoryCache.TryGetValue(key, out actual);
			}
		}

		Assert.True(ok);
		Assert.Equal(value, actual);
	}

	[Theory]
	[InlineData(1_665_487_302, "f0:2f:74:d2:09:a5", "192.168.1.229", "malik10", "01:f0:2f:74:d2:09:a5")]
	public void CacheEntireDhcpLease(int expiry, string macString, string ipString, string hostName, string identifer)
	{
		var lease = new Helpers.Networking.Models.DhcpLease(
			DateTime.UnixEpoch.AddSeconds(expiry),
			PhysicalAddress.Parse(macString),
			IPAddress.Parse(ipString),
			hostName,
			identifer);

		using IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

		memoryCache.Set(lease.IPAddress, lease);
		memoryCache.Set(lease.PhysicalAddress, lease);

		if (!string.IsNullOrWhiteSpace(lease.HostName))
		{
			memoryCache.Set(lease.HostName, lease);
		}

		Thread.Sleep(millisecondsTimeout: 1_000);

		Assert.True(memoryCache.TryGetValue(lease.IPAddress, out _));
		Assert.True(memoryCache.TryGetValue(lease.PhysicalAddress, out _));

		if (!string.IsNullOrWhiteSpace(lease.HostName))
		{
			Assert.True(memoryCache.TryGetValue(lease.HostName, out _));
		}
	}

	[Theory]
	[InlineData("f02f74d209a5", "f02f74d209a5")]
	[InlineData("f02f74d209a5", "F02F74D209A5")]
	[InlineData("f02f74d209a5", "f0:2f:74:d2:09:a5")]
	[InlineData("f02f74d209a5", "F0:2F:74:D2:09:A5")]
	[InlineData("F02F74D209A5", "f02f74d209a5")]
	[InlineData("F02F74D209A5", "F02F74D209A5")]
	[InlineData("F02F74D209A5", "f0:2f:74:d2:09:a5")]
	[InlineData("F02F74D209A5", "F0:2F:74:D2:09:A5")]
	[InlineData("f0:2f:74:d2:09:a5", "f02f74d209a5")]
	[InlineData("f0:2f:74:d2:09:a5", "F02F74D209A5")]
	[InlineData("f0:2f:74:d2:09:a5", "f0:2f:74:d2:09:a5")]
	[InlineData("f0:2f:74:d2:09:a5", "F0:2F:74:D2:09:A5")]
	[InlineData("F0:2F:74:D2:09:A5", "f02f74d209a5")]
	[InlineData("F0:2F:74:D2:09:A5", "F02F74D209A5")]
	[InlineData("F0:2F:74:D2:09:A5", "f0:2f:74:d2:09:a5")]
	[InlineData("F0:2F:74:D2:09:A5", "F0:2F:74:D2:09:A5")]
	public void DictionaryWithPhysicalAddressAsKeyTests(params string[] keysStrings)
	{
		var value = "hello world";
		var dictionary = new Dictionary<PhysicalAddress, string>();
		var keys = keysStrings.Select(PhysicalAddress.Parse).ToArray();

		dictionary.Add(keys[0], value);
		var ok = dictionary.TryGetValue(keys[1], out var actual);

		Assert.True(ok);
		Assert.Equal(value, actual);
	}

	[Theory]
	[InlineData("f02f74d209a5", "f02f74d209a5")]
	[InlineData("f02f74d209a5", "F02F74D209A5")]
	[InlineData("f02f74d209a5", "f0:2f:74:d2:09:a5")]
	[InlineData("f02f74d209a5", "F0:2F:74:D2:09:A5")]
	[InlineData("F02F74D209A5", "f02f74d209a5")]
	[InlineData("F02F74D209A5", "F02F74D209A5")]
	[InlineData("F02F74D209A5", "f0:2f:74:d2:09:a5")]
	[InlineData("F02F74D209A5", "F0:2F:74:D2:09:A5")]
	[InlineData("f0:2f:74:d2:09:a5", "f02f74d209a5")]
	[InlineData("f0:2f:74:d2:09:a5", "F02F74D209A5")]
	[InlineData("f0:2f:74:d2:09:a5", "f0:2f:74:d2:09:a5")]
	[InlineData("f0:2f:74:d2:09:a5", "F0:2F:74:D2:09:A5")]
	[InlineData("F0:2F:74:D2:09:A5", "f02f74d209a5")]
	[InlineData("F0:2F:74:D2:09:A5", "F02F74D209A5")]
	[InlineData("F0:2F:74:D2:09:A5", "f0:2f:74:d2:09:a5")]
	[InlineData("F0:2F:74:D2:09:A5", "F0:2F:74:D2:09:A5")]
	public void PhysicalAddressUniquenessTests(params string[] keysStrings)
	{
		var keys = keysStrings.Select(PhysicalAddress.Parse).ToArray();

		Assert.False(ReferenceEquals(keys[0], keys[1]));
		//Assert.True(keys[0] == keys[1]);
		Assert.Equal(keys[0], keys[1]);
		Assert.True(keys[0].Equals(keys[1]));
		Assert.True(Equals(keys[0], keys[1]));
	}
}
