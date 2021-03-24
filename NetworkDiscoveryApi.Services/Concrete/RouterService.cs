using Dawn;
using Helpers.SSH.Services;
using System.Collections.Generic;

namespace NetworkDiscoveryApi.Services.Concrete
{
	public class RouterService : IRouterService
	{
		private readonly ISSHService _sshService;

		public RouterService(ISSHService sshService)
		{
			_sshService = Guard.Argument(() => sshService).NotNull().Value;
		}

		public IAsyncEnumerable<Helpers.Networking.Models.DhcpEntry> GetDhcpLeasesAsync()
			=> _sshService.GetDhcpLeasesAsync();

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
