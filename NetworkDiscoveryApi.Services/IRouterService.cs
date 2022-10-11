using Dawn;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.Services;

public interface IRouterService
{
	Helpers.Networking.Models.DhcpLease GetLeaseByAlias(string alias)
	{
		Guard.Argument(alias).NotNull().NotEmpty().NotWhiteSpace();
		return GetLease(alias);
	}

	Helpers.Networking.Models.DhcpLease GetLeaseByIPAddress(IPAddress ip)
	{
		Guard.Argument(ip).NotNull().Require(o => !Equals(o, IPAddress.None));
		return GetLease(ip);
	}

	Helpers.Networking.Models.DhcpLease GetLeaseByPhysicalAddress(PhysicalAddress mac)
	{
		Guard.Argument(mac).NotNull().Require(o => !Equals(o, PhysicalAddress.None));
		return GetLease(mac);
	}

	Helpers.Networking.Models.DhcpLease GetLease(object key);
}
