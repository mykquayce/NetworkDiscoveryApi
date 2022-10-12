using System.Net.NetworkInformation;
using Xunit;

namespace NetworkDiscoveryApi.WebApplication.Tests;

public class DictionaryTests
{
	[Fact]
	public void Test1()
	{
		var dictionary = new Dictionary<string, PhysicalAddress>(StringComparer.OrdinalIgnoreCase)
		{
			["vr front"] = PhysicalAddress.Parse("28ee52eb0aa4"),
			["vr rear"] = PhysicalAddress.Parse("003192e1a474"),
			["philipshue"] = PhysicalAddress.Parse("ecb5fa18e324"),
			["philipshuebridge"] = PhysicalAddress.Parse("ecb5fa18e324"),
			["globalcache"] = PhysicalAddress.Parse("000c1e059cad"),
			["irblaster"] = PhysicalAddress.Parse("000c1e059cad"),
		};

		var actual = dictionary.Invert();

		Assert.NotEmpty(actual);
		Assert.Equal(4, actual.Count);
	}
}
