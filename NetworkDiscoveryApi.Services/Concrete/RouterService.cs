using Dawn;
using Helpers.SSH.Services;
using NetworkDiscoveryApi.Models;
using System.Collections.Generic;

namespace NetworkDiscoveryApi.Services.Concrete
{
	public class RouterService : IRouterService
	{
		private readonly ISSHService _sshService;
		private readonly ICachingService<IList<DhcpEntry>> _cachingService;

		public RouterService(ISSHService sshService, ICachingService<IList<DhcpEntry>> cachingService)
		{
			_sshService = Guard.Argument(() => sshService).NotNull().Value;
			_cachingService = Guard.Argument(() => cachingService).NotNull().Value;
		}

		public async IAsyncEnumerable<DhcpEntry> GetDhcpLeasesAsync()
		{
			if (!_cachingService.TryGet(out var entries))
			{
				entries = new List<DhcpEntry>();

				await foreach (var item in _sshService.GetDhcpLeasesAsync())
				{
					var entry = (DhcpEntry)item;

					entries.Add(entry);
				}

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
