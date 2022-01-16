using Microsoft.AspNetCore.Mvc.Testing;

namespace NetworkDiscoveryApi.WebApplication.Tests.Fixtures;

public sealed class WebHostFixture : IDisposable
{
	private readonly WebApplicationFactory<Program> _factory;

	public WebHostFixture()
	{
		_factory = new();
		HttpClient = _factory.CreateClient();
	}

	public HttpClient HttpClient { get; }

	#region dispose
	public void Dispose()
	{
		HttpClient.Dispose();
		_factory.Dispose();
	}
	#endregion dispose
}
