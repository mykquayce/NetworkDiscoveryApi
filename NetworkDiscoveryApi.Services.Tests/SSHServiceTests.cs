using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace NetworkDiscoveryApi.Services.Tests;

public class SSHServiceTests
{
	[Theory]
	[InlineData("192.168.1.10", 22, "root", "5d2d7995d1b846695bba6d54291557bf56d084dab71ddb7df3777e6259dc6655")]
	public async Task Test1(string host, int port, string username, string password)
	{
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var initialData = new Dictionary<string, string?>
				{
					[nameof(host)] = host,
					[nameof(port)] = port.ToString(),
					[nameof(username)] = username,
					[nameof(password)] = password,
				};

				configuration = new ConfigurationBuilder()
					.AddInMemoryCollection(initialData)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.AddSSH(configuration)
				.BuildServiceProvider();
		}

		var sut = serviceProvider.GetRequiredService<Helpers.SSH.IService>();

		var leases = await sut.GetDhcpLeasesAsync().ToListAsync();

		Assert.NotEmpty(leases);
		Assert.DoesNotContain(default, leases);

		if (serviceProvider is ServiceProvider disposable) await disposable.DisposeAsync();
	}
}

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddSSH(this IServiceCollection services, string host, int port, string username, string? password = null, string? pathToPrivateKey = null)
	{
		var config = new Helpers.SSH.Config(host, port, username, password, pathToPrivateKey);
		return services
			.AddSingleton(Options.Create(config))
			.AddSSH();
	}

	public static IServiceCollection AddSSH(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.Configure<Helpers.SSH.Config>(configuration)
			.AddSSH();
	}

	public static IServiceCollection AddSSH(this IServiceCollection services)
	{
		return services
			.AddScoped<Helpers.SSH.IClient, Helpers.SSH.Concrete.Client>()
			.AddScoped<Helpers.SSH.IService, Helpers.SSH.Concrete.Service>();
	}
}