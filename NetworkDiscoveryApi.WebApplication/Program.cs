using Microsoft.Extensions.Caching.Memory;
using NetworkDiscoveryApi.Services;
using NetworkDiscoveryApi.Services.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
	.AddHostedService<NetworkDiscoveryApi.WebApplication.Worker>()
	.AddSingleton<NetworkDiscoveryApi.WebApplication.ICustomWorkerStarter, NetworkDiscoveryApi.WebApplication.Worker>();

builder.Services
	.AddAuthenticationAuthorization(builder.Configuration.GetSection("Identity"));

builder.Services
	.AddSSH(builder.Configuration.GetSection("Router"));

builder.Services
	.AddEnumerableMemoryCache(new MemoryCacheOptions())
	.AddTransient<IRouterService, RouterService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
	.RequireAuthorization("ApiScope");

app.Run();

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1050:Declare types in namespaces", Justification = "needed by integration tests")]
public partial class Program { }
