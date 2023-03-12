using IdentityModel.Client;
using System.Text.Json;
using System.Web;
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

	[Theory]
	[InlineData("7e:a7:b0:32:8b:3e")]
	[InlineData("7E:A7:B0:32:8B:3E")]
	[InlineData("7ea7b0328b3e")]
	[InlineData("7e-a7-b0-32-8b-3e")]
	[InlineData("7EA7B0328B3E")]
	[InlineData("7E-A7-B0-32-8B-3E")]
	public async Task GetDhcpLicenseForMac(string physicalAddressString)
	{
		_httpClient.SetBearerToken(AccessToken);

		var response = await _httpClient.GetStringAsync("api/router/" + HttpUtility.UrlPathEncode(physicalAddressString));

		Assert.NotNull(response);
		Assert.NotEmpty(response);
		Assert.StartsWith("{", response);
		Assert.NotEqual("{}", response);
	}

	[Theory]
	[InlineData("api/router")]
	public async Task GetAllDhcpLeases(string requestUri)
	{
		_httpClient.SetBearerToken(AccessToken);

		var response = await _httpClient.GetStringAsync(requestUri);

		Assert.NotNull(response);
		Assert.NotEmpty(response);
		Assert.StartsWith("[", response);
		Assert.NotEqual("[]", response);

		ICollection<string> aliases = JsonSerializer.Deserialize<string[]>(response)!;

		Assert.NotEmpty(aliases);
		Assert.All(aliases, Assert.NotNull);
		Assert.All(aliases, Assert.NotEmpty);
	}
}
