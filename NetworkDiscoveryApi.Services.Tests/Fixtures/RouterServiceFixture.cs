namespace NetworkDiscoveryApi.Services.Tests.Fixtures;

public sealed class RouterServiceFixture : IDisposable
{
	private readonly MemoryCacheFixture _memoryCacheFixture = new();

	public RouterServiceFixture()
	{
		RouterService = new Concrete.RouterService(_memoryCacheFixture.MemoryCache);
	}

	public IRouterService RouterService { get; }

	public void Dispose() => _memoryCacheFixture.Dispose();
}
