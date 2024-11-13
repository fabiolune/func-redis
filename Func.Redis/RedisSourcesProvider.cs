namespace Func.Redis;

public class RedisSourcesProvider(IConnectionMultiplexerProvider provider) : ISourcesProvider
{
    private readonly IConnectionMultiplexerProvider _provider = provider;

    public IDatabase GetDatabase() =>
        _provider
            .GetMultiplexer()
            .GetDatabase();

    public IServer[] GetServers() =>
        _provider
            .GetMultiplexer()
            .GetServers();
}