using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace NetworkDiscoveryApi.WebApplication.Tests.Fixtures;

public sealed class WebHostFixture : IDisposable
{
	public WebHostFixture()
	{
		var builder = new WebHostBuilder()
			.UseStartup<Startup>()
			.ConfigureAppConfiguration(config =>
			{
				config
					.AddEnvironmentVariables()
					.AddUserSecrets(this.GetType().Assembly, optional: true);
			});

		Server = new TestServer(builder);
		HttpClient = Server.CreateClient();
	}

	public TestServer Server { get; }
	public HttpClient HttpClient { get; }

	#region dispose
	public void Dispose()
	{
		HttpClient.Dispose();
		Server.Dispose();
	}
	#endregion dispose
}
