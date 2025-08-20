namespace Func.Redis.Tests;

public class RedisSourcesProviderTests
{
    [Test]
    public void GetDatabase_ShouldReturnMultiplexerDatabase()
    {
        var mockProvider = Substitute.For<IConnectionMultiplexerProvider>();
        var mockMultiplexer = Substitute.For<IConnectionMultiplexer>();
        var mockDatabase = Substitute.For<IDatabase>();
        mockProvider
            .GetMultiplexer()
            .Returns(mockMultiplexer);
        mockMultiplexer
            .GetDatabase(-1, null)
            .Returns(mockDatabase);

        var sut = new RedisSourcesProvider(mockProvider);

        sut
            .GetDatabase()
            .ShouldBe(mockDatabase);
    }

    [Test]
    public void GetServers_ShouldReturnMultiplexerServers()
    {
        var mockProvider = Substitute.For<IConnectionMultiplexerProvider>();
        var mockMultiplexer = Substitute.For<IConnectionMultiplexer>();

        var server1 = Substitute.For<IServer>();
        var server2 = Substitute.For<IServer>();
        var servers = new[] { server1, server2 };

        mockProvider
            .GetMultiplexer()
            .Returns(mockMultiplexer);
        mockMultiplexer
            .GetServers()
            .Returns(servers);

        var sut = new RedisSourcesProvider(mockProvider);
        
        var result = sut.GetServers();
        result.Length.ShouldBe(2);
        result[0].ShouldBe(server1);
        result[1].ShouldBe(server2);
    }
}