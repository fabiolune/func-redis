using Testcontainers.Redis;
using RedisConfiguration = Func.Redis.Models.RedisConfiguration;

namespace Func.Redis.IntegrationTests;
internal abstract class RedisIntegrationTestBase(string redisImage)
{
    private RedisContainer _container;
    private readonly string _redisImage = redisImage;
    protected RedisSourcesProvider _provider;
    protected IConnectionMultiplexerProvider _connectionMultiplexerProvider;

    [OneTimeSetUp]
    public virtual async Task OneTimeSetUp()
    {
        _container = new RedisBuilder()
            .WithImage(_redisImage)
            .Build();

        await _container.StartAsync();

        _connectionMultiplexerProvider = _container
            .Map(c => $"{c.Hostname}:{c.GetMappedPublicPort(6379)}")
            .Map(cs => new RedisConfiguration
            {
                ConnectionString = cs
            })
            .Map(c => new ConnectionMultiplexerProvider(c));

        _provider = _connectionMultiplexerProvider
            .Map(p => new RedisSourcesProvider(p));
    }

    [OneTimeTearDown]
    public Task OneTimeTearDown() => _container.StopAsync();
}
