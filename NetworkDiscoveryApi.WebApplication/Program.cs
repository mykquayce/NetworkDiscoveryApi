using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace NetworkDiscoveryApi.WebApplication
{
	public static class Program
	{
		public static Task Main(string[] args) => CreateHostBuilder(args).RunConsoleAsync();

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			var hostBuilder = Host.CreateDefaultBuilder(args);

			hostBuilder
				.ConfigureAppConfiguration((context, configBuilder) =>
				{
					configBuilder
						.AddDockerSecrets(optional: true, reloadOnChange: true)
						.AddUserSecrets(typeof(Program).Assembly, optional: true, reloadOnChange: true);
				});

			hostBuilder
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});

			return hostBuilder;
		}
	}
}
