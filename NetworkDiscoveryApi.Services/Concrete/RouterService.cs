using Dawn;
using Microsoft.Extensions.Options;
using NetworkDiscoveryApi.Services.Models;
using System.Net.NetworkInformation;

namespace NetworkDiscoveryApi.Services.Concrete;

public class RouterService : IRouterService
{
	private readonly Helpers.SSH.IService _sshService;
	private readonly IReadOnlyDictionary<string, PhysicalAddress> _aliasesLookup;

	public RouterService(
		Helpers.SSH.IService sshService,
		IOptions<IReadOnlyDictionary<string, PhysicalAddress>> aliasesOptions)
	{
		_sshService = Guard.Argument(sshService).NotNull().Value;
		_aliasesLookup = Guard.Argument(aliasesOptions).NotNull().Wrap(o => o.Value)
			.NotNull().NotEmpty().Value;
	}

	public async IAsyncEnumerable<DhcpLease> GetLeasesAsync()
	{
		var leases = _sshService.GetDhcpLeasesAsync();

		var aliasesLookup = _aliasesLookup.Invert();

		await foreach (var lease in leases)
		{
			var (expiry, mac, ip, hostname, _) = lease;

			var ok = aliasesLookup.TryGetValue(mac, out var aliases);

			if (ok)
			{
				foreach (var alias in aliases!)
				{
					yield return new(expiry, mac, ip, hostname?.ToLowerInvariant(), alias.ToLowerInvariant());
				}
			}
			else
			{
				yield return new(expiry, mac, ip, hostname?.ToLowerInvariant(), Alias: null);
			}
		}
	}
}
