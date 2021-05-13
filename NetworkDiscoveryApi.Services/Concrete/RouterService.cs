using Dawn;
using Helpers.SSH.Services;
using System.Collections.Generic;
using System.Linq;

namespace NetworkDiscoveryApi.Services.Concrete
{
	public class RouterService : IRouterService
	{
		private readonly ISSHService _sshService;
		private readonly ICachingService<IList<Helpers.Networking.Models.DhcpLease>> _cachingService;

		public RouterService(ISSHService sshService, ICachingService<IList<Helpers.Networking.Models.DhcpLease>> cachingService)
		{
			_sshService = Guard.Argument(() => sshService).NotNull().Value;
			_cachingService = Guard.Argument(() => cachingService).NotNull().Value;
		}

		public async IAsyncEnumerable<Helpers.Networking.Models.DhcpLease> GetDhcpLeasesAsync()
		{
			if (!_cachingService.TryGet(out var entries))
			{
				entries = await _sshService.GetDhcpLeasesAsync().ToListAsync();

				_cachingService.Set(entries);
			}

			foreach (var entry in entries!)
			{
				yield return entry;
			}
		}

		#region Dispose
		private bool _disposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_sshService.Dispose();
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			System.GC.SuppressFinalize(this);
		}
		#endregion Dispose
	}
}
