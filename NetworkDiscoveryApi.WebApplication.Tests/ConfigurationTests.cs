using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;
using System.Text;
using Xunit;

namespace NetworkDiscoveryApi.WebApplication.Tests;

public class ConfigurationTests
{
	[Theory]
	[InlineData(@"{
	""vr front"": ""28ee52eb0aa4"",
	""vr rear"": ""003192e1a474"",
	""philipshue"": ""ecb5fa18e324"",
	""philipshuebridge"": ""ecb5fa18e324"",
	""globalcache"": ""000c1e059cad"",
	""irblaster"": ""000c1e059cad""
}")]
	public void Test1(string json)
	{
		IConfiguration configuration;
		{
			var bytes = Encoding.UTF8.GetBytes(json);
			using var stream = new MemoryStream(bytes);

			configuration = new ConfigurationBuilder()
				.AddJsonStream(stream)
				.Build();
		}

		using var serviceProvider = new ServiceCollection()
			//.Configure<Dictionary<string, string[]>>(configuration)
			.AddSingleton<IOptions<IReadOnlyDictionary<string, PhysicalAddress>>>(provider =>
			{
				var dictionary = new Dictionary<string, string>();
				configuration.Bind(dictionary);
				var aliases = dictionary.ToDictionary(
						kvp => kvp.Key,
						kvp => PhysicalAddress.Parse(kvp.Value),
						StringComparer.OrdinalIgnoreCase)
					.AsReadOnly();
				return Options.Create(aliases);
			})
			.BuildServiceProvider();

		var actual = serviceProvider.GetRequiredService<IOptions<IReadOnlyDictionary<string, PhysicalAddress>>>();

		Assert.NotNull(actual);
		Assert.NotNull(actual.Value);
		Assert.NotEmpty(actual.Value);
	}
}
