using Dawn;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.Services;

public interface IMemoryCacheService<T>
{
	void Clear();
	T Get(object key);
	void Set(object key, T value, DateTime expiration);

	T GetLeaseByAlias(string alias)
	{
		Guard.Argument(alias).NotNull().NotEmpty().NotWhiteSpace();
		return Get(alias);
	}

	T GetLeaseByIPAddress(IPAddress ip)
	{
		Guard.Argument(ip).NotNull().Require(o => !Equals(o, IPAddress.None));
		return Get(ip);
	}

	T GetLeaseByPhysicalAddress(PhysicalAddress mac)
	{
		Guard.Argument(mac).NotNull().Require(o => !Equals(o, PhysicalAddress.None));
		return Get(mac);
	}
}
