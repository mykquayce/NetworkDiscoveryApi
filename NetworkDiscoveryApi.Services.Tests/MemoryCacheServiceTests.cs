using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace NetworkDiscoveryApi.Services.Tests;

public class MemoryCacheServiceTests
{
	[Theory]
	[InlineData("key", "value")]
	public void ClearTests(object key, string value)
	{
		// Arrange
		using IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
		IMemoryCacheService<string> sut = new Concrete.MemoryCacheService<string>(memoryCache);
		sut.Set(key, value, DateTime.UtcNow.AddMinutes(1));

		// Act, Assert
		testcode();
		sut.Clear();
		Assert.Throws<ArgumentOutOfRangeException>(testcode);

		void testcode() => sut.Get(key);
	}
}
