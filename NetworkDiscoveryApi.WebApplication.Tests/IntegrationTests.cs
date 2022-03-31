using IdentityModel.Client;
using Xunit;

namespace NetworkDiscoveryApi.WebApplication.Tests;

public class IntegrationTests : IClassFixture<Fixtures.WebHostFixture>, IClassFixture<Fixtures.IdentityClientFixture>
{
	private readonly HttpClient _httpClient;
	private readonly Helpers.Identity.Clients.IIdentityClient _identityClient;

	public IntegrationTests(Fixtures.WebHostFixture webHostFixture, Fixtures.IdentityClientFixture identityClientFixture)
	{
		_httpClient = webHostFixture.HttpClient;
		_identityClient = identityClientFixture.IdentityClient;
	}

	[Fact]
	public async Task GetDhcpLicenses()
	{
		var accessToken = await _identityClient.GetAccessTokenAsync();

		_httpClient.SetBearerToken(accessToken);

		var response = await _httpClient.GetStringAsync("/api/router");

		Assert.NotNull(response);
		Assert.NotEmpty(response);
		Assert.StartsWith("[", response);
	}
}
