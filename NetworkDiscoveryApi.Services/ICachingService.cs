using System;

namespace NetworkDiscoveryApi.Services
{
	public interface ICachingService<T> : IDisposable
		where T : class
	{
		void Set(T value);
		bool TryGet(out T? value);
	}
}
