using Dawn;
using Microsoft.Extensions.Caching.Memory;

namespace NetworkDiscoveryApi.Services.Concrete;

public class RouterService : IRouterService
{
	private readonly IMemoryCache _memoryCache;

	public RouterService(IMemoryCache memoryCache)
	{
		_memoryCache = Guard.Argument(() => memoryCache).NotNull().Value;
	}

	public Helpers.Networking.Models.DhcpLease GetLease(object key)
	{
		if (_memoryCache.TryGetValue<Helpers.Networking.Models.DhcpLease>(key, out var lease))
		{
			return lease!;
		}

		throw new ArgumentOutOfRangeException(nameof(key), key, $"{nameof(key)} {key} not found");
	}
}
