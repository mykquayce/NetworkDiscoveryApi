using Xunit;

namespace NetworkDiscoveryApi.WebApplication.Tests;

public class IntegrationTests : IClassFixture<Fixtures.WebHostFixture>
{
	private readonly HttpClient _httpClient;

	public IntegrationTests(Fixtures.WebHostFixture fixture)
	{
		_httpClient = fixture.HttpClient;
	}

	[Fact]
	public async Task GetDhcpLicenses()
	{
		var response = await _httpClient.GetStringAsync("/api/router");

		Assert.NotNull(response);
		Assert.NotEmpty(response);
		Assert.Equal('[', response[0]);
	}
}
