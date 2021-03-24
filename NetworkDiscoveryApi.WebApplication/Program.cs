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
						.AddUserSecrets(typeof(Program).Assembly);
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
