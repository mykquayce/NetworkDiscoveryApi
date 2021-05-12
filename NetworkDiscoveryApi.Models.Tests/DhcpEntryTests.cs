using System;
using System.Globalization;
using System.Text.Json;
using Xunit;

namespace NetworkDiscoveryApi.Models.Tests
{
	public class DhcpEntryTests
	{
		private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true, };

		[Theory]
		[InlineData("{\"expiration\":\"2021-03-24T22:47:22.0000000Z\",\"hostName\":\"2two\",\"identifier\":\"01:00:15:5d:01:da:00\",\"ipAddress\":\"192.168.1.101\",\"physicalAddress\":\"00155d01da00\"}")]
		public void Deserialize(string json)
		{
			var entry = JsonSerializer.Deserialize<DhcpEntry>(json, _options);

			Assert.NotNull(entry);
			Assert.NotNull(entry!.Expiration);
			Assert.NotNull(entry.IPAddress);
			Assert.NotNull(entry.PhysicalAddress);
		}

		[Theory]
		[InlineData("[{\"expiration\":\"2021-03-24T22:47:22.0000000Z\",\"hostName\":\"2two\",\"identifier\":\"01:00:15:5d:01:da:00\",\"ipAddress\":\"192.168.1.101\",\"physicalAddress\":\"00155d01da00\"},{\"expiration\":\"2021-03-25T03:19:07.0000000Z\",\"hostName\":\"johnson\",\"identifier\":\"01:00:c2:c6:cb:35:31\",\"ipAddress\":\"192.168.1.133\",\"physicalAddress\":\"00c2c6cb3531\"},{\"expiration\":\"2021-03-25T04:26:31.0000000Z\",\"hostName\":\"jensen7\",\"identifier\":\"01:9c:5c:8e:bd:e5:d6\",\"ipAddress\":\"192.168.1.218\",\"physicalAddress\":\"9c5c8ebde5d6\"},{\"expiration\":\"2021-03-25T04:35:52.0000000Z\",\"hostName\":\"XBOX\",\"identifier\":\"01:4c:3b:df:97:98:3e\",\"ipAddress\":\"192.168.1.201\",\"physicalAddress\":\"4c3bdf97983e\"},{\"expiration\":\"2021-03-25T03:09:07.0000000Z\",\"hostName\":\"flichub\",\"identifier\":\"ff:b0:32:8b:3e:00:01:00:01:c7:92:bc:90:da:fc:ce:74:4e:01\",\"ipAddress\":\"192.168.1.214\",\"physicalAddress\":\"7ea7b0328b3e\"},{\"expiration\":\"2021-03-25T02:56:05.0000000Z\",\"hostName\":\"HS110\",\"identifier\":null,\"ipAddress\":\"192.168.1.231\",\"physicalAddress\":\"b09575e4f988\"},{\"expiration\":\"2021-03-25T01:57:49.0000000Z\",\"hostName\":null,\"identifier\":null,\"ipAddress\":\"192.168.1.217\",\"physicalAddress\":\"3c6a9d14d765\"},{\"expiration\":\"2021-03-24T23:58:30.0000000Z\",\"hostName\":\"iTach059CAD\",\"identifier\":null,\"ipAddress\":\"192.168.1.116\",\"physicalAddress\":\"000c1e059cad\"},{\"expiration\":\"2021-03-25T05:41:56.0000000Z\",\"hostName\":null,\"identifier\":null,\"ipAddress\":\"192.168.1.111\",\"physicalAddress\":\"a438ccdecbe2\"},{\"expiration\":\"2021-03-25T02:28:14.0000000Z\",\"hostName\":\"Philips-hue\",\"identifier\":null,\"ipAddress\":\"192.168.1.156\",\"physicalAddress\":\"ecb5fa18e324\"}]")]
		public void DeserializeMany(string json)
		{
			var entries = JsonSerializer.Deserialize<DhcpEntry[]>(json, _options);

			Assert.NotNull(entries);
			Assert.NotEmpty(entries);

			foreach (var (expiration, _, _, ipAddress, physicalAddress) in entries!)
			{
				Assert.NotNull(expiration);
				Assert.NotNull(ipAddress);
				Assert.NotNull(physicalAddress);
			}
		}

		[Theory]
		[InlineData("{\"expiration\":\"2021-03-24T22:47:22.0000000Z\",\"hostName\":\"2two\",\"identifier\":\"01:00:15:5d:01:da:00\",\"ipAddress\":\"192.168.1.101\",\"physicalAddress\":\"00155d01da00\"}")]
		public void Cast(string json)
		{
			var before = JsonSerializer.Deserialize<DhcpEntry>(json, _options);

			Assert.NotNull(before);

			var after = (Helpers.Networking.Models.DhcpLease)before!;

			var before2 = (DhcpEntry)after;

			Assert.False(ReferenceEquals(before, before2));
			Assert.Equal(before, before2);
		}

		[Theory]
		[InlineData("2021-03-24T22:47:22.0000000Z")]
		public void DateTimeSerializeDeserialize(string s)
		{
			var cultureInfo = CultureInfo.InvariantCulture;
			var style = DateTimeStyles.AdjustToUniversal;
			var dt = DateTime.Parse(s, cultureInfo, style);
			var after = dt.ToString("O");
			Assert.Equal(s, after);
		}
	}
}
