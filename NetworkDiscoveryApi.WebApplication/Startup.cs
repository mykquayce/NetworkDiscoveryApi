using Microsoft.OpenApi.Models;

namespace NetworkDiscoveryApi.WebApplication;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
	{
		services
			.Configure<Helpers.SSH.Config>(Configuration.GetSection("Router"));

		services
			.AddSingleton<Services.ICachingService<IList<Helpers.Networking.Models.DhcpLease>>, Services.Concrete.CachingService<IList<Helpers.Networking.Models.DhcpLease>>>()
			.AddTransient<Helpers.SSH.IClient, Helpers.SSH.Concrete.Client>()
			.AddTransient<Helpers.SSH.IService, Helpers.SSH.Concrete.Service>()
			.AddTransient<Services.IRouterService, Services.Concrete.RouterService>();

		services.AddControllers();
		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "NetworkDiscoveryApi.WebApplication", Version = "v1" });
		});
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetworkDiscoveryApi.WebApplication v1"));
		}

		app.UseRouting();

		app.UseAuthorization();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
	}
}
