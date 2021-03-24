using System;
using System.Runtime.Caching;

namespace NetworkDiscoveryApi.Services.Concrete
{
	public class CachingService<T> : ICachingService<T> where T : class
	{
		private readonly string _key = typeof(T).Name;
		private readonly MemoryCache _cache = MemoryCache.Default;
		private readonly TimeSpan _expires = TimeSpan.FromHours(1);

		public bool TryGet(out T? value)
		{
			value = _cache[_key] as T;
			return value is not null;
		}

		public void Set(T value)
		{
			var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow + _expires, };
			_cache.Set(_key, value, policy);
		}

		#region disposable
		private bool _disposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_cache.Dispose();
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion disposable
	}
}
