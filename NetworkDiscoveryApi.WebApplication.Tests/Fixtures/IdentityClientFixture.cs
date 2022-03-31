using Microsoft.Extensions.Caching.Memory;

namespace NetworkDiscoveryApi.WebApplication.Tests.Fixtures;

public sealed class IdentityClientFixture : IDisposable
{
	private static readonly Uri _authority = new("https://identityserver");
	private const string _cliendId = "elgatoapi";
	private const string _clientSecret = "8556e52c6ab90d042bb83b3f0c8894498beeb65cf908f519a2152aceb131d3ee";
	private const string _scope = "networkdiscovery";

	private readonly HttpClient _httpClient;
	private readonly IMemoryCache _memoryCache;

	public IdentityClientFixture()
	{
		var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		_httpClient = new HttpClient(httpClientHandler) { BaseAddress = _authority, };
		var config = new Helpers.Identity.Config(_authority, _cliendId, _clientSecret, _scope);
		_memoryCache = new MemoryCache(new MemoryCacheOptions());

		IdentityClient = new Helpers.Identity.Clients.Concrete.IdentityClient(config, _httpClient, _memoryCache);
	}

	public Helpers.Identity.Clients.IIdentityClient IdentityClient { get; }

	#region Dispose
	public void Dispose()
	{
		_httpClient.Dispose();
		_memoryCache.Dispose();
	}
	#endregion Dispose
}
