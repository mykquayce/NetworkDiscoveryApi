using Dawn;
using Microsoft.Extensions.Caching.Memory;

namespace NetworkDiscoveryApi.Services.Concrete;

public class MemoryCacheService<T> : IMemoryCacheService<T>
{
	private readonly IMemoryCache _memoryCache;

	public MemoryCacheService(IMemoryCache memoryCache)
	{
		_memoryCache = memoryCache;
	}

	public void Clear() => (_memoryCache as MemoryCache)?.Clear();

	public T Get(object key)
	{
		Guard.Argument(key).NotNull();

		if (_memoryCache.TryGetValue<T>(key, out var value))
		{
			return value!;
		}

		throw new ArgumentOutOfRangeException(nameof(key), key, $"{nameof(key)} {key} not found");
	}

	public void Set(object key, T value, DateTime expiration)
	{
		Guard.Argument(key).NotNull();
		Guard.Argument(expiration).InRange(DateTime.UtcNow.AddMinutes(-1), DateTime.MaxValue);

		_memoryCache.Set(key, value, expiration);
	}
}
