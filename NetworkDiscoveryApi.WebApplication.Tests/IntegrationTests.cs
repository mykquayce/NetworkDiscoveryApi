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

	public string? _accessToken;
	public string AccessToken => _accessToken ??= _identityClient.GetAccessTokenAsync().GetAwaiter().GetResult();

	[Fact]
	public async Task GetDhcpLicenses()
	{
		_httpClient.SetBearerToken(AccessToken);

		var response = await _httpClient.GetStringAsync("/api/router");

		Assert.NotNull(response);
		Assert.NotEmpty(response);
		Assert.StartsWith("[", response);
	}

	[Theory]
	[InlineData("3c:6a:9d:14:d7:65")]
	[InlineData("3C:6A:9D:14:D7:65")]
	[InlineData("3c6a9d14d765")]
	[InlineData("3c-6a-9d-14-d7-65")]
	[InlineData("3C6A9D14D765")]
	[InlineData("3C-6A-9D-14-D7-65")]
	public async Task GetDhcpLicenseForMac(string physicalAddressString)
	{
		_httpClient.SetBearerToken(AccessToken);

		var response = await _httpClient.GetStringAsync("/api/router/" + physicalAddressString);

		Assert.NotNull(response);
		Assert.NotEmpty(response);
		Assert.StartsWith("{", response);
	}
}
