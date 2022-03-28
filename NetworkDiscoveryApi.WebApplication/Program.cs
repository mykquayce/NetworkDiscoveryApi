var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
	.AddAuthenticationAuthorization(builder.Configuration.GetSection("Identity"));

builder.Services
	.Configure<Helpers.SSH.Config>(builder.Configuration.GetSection("Router"));

builder.Services
	.AddSingleton<NetworkDiscoveryApi.Services.ICachingService<IList<Helpers.Networking.Models.DhcpLease>>, NetworkDiscoveryApi.Services.Concrete.CachingService<IList<Helpers.Networking.Models.DhcpLease>>>()
	.AddTransient<Helpers.SSH.IClient, Helpers.SSH.Concrete.Client>()
	.AddTransient<Helpers.SSH.IService, Helpers.SSH.Concrete.Service>()
	.AddTransient<NetworkDiscoveryApi.Services.IRouterService, NetworkDiscoveryApi.Services.Concrete.RouterService>();

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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
	.RequireAuthorization("ApiScope");

app.Run();

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1050:Declare types in namespaces", Justification = "needed by integration tests")]
public partial class Program { }
