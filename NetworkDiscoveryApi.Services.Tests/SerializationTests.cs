using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json;
using Xunit;

namespace NetworkDiscoveryApi.Services.Tests;

public class SerializationTests
{
	[Theory]
	[InlineData(1_620_891_591, "000c1e059cad", "192.168.1.121", "iTach059CAD", "*")]
	public void DhcpLease(int expiration, string physicalAddressString, string ipAddressString, string hostName, string identifier)
	{
		var lease = new Helpers.Networking.Models.DhcpLease(
			DateTime.UnixEpoch.AddSeconds(expiration),
			PhysicalAddress.Parse(physicalAddressString),
			IPAddress.Parse(ipAddressString),
			hostName,
			identifier);

		var json = JsonSerializer.Serialize(lease);

		Assert.NotNull(json);
		Assert.NotEmpty(json);
		Assert.NotEqual("{}", json);

		var actual = JsonSerializer.Deserialize<Dictionary<string, string?>>(json)!
			.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase)
			.AsReadOnly();

		Assert.NotNull(actual);
		foreach (var key in new[] { "expiration", "physicaladdress", "ipaddress", "hostname", "identifier", })
		{
			Assert.Contains(key, actual.Keys, StringComparer.OrdinalIgnoreCase);
			var value = actual[key];
			Assert.NotNull(value);
			Assert.NotEmpty(value);
		}
	}
}
