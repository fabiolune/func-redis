using NSubstitute.ExceptionExtensions;

namespace Func.Redis.Tests.RedisKeyService;
public partial class RedisKeyServiceTests
{
    [Test]
    public async Task GetKeysAsync_WhenMultiplexerThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        _mockSourcesProvider
            .GetServers()
            .Throws(exception);

        var result = await _sut.GetKeysAsync("some pattern");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(Error.New(exception)));
    }

    [Test]
    public async Task GetKeysAsync_WhenServerThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");

        var server1 = Substitute.For<IServer>();
        var server2 = Substitute.For<IServer>();

        var servers = new[]
        {
            server1,
            server2
        };

        _mockSourcesProvider
            .GetServers()
            .Returns(servers);

        server1
            .KeysAsync(pattern: "some pattern")
            .Throws(exception);

        var keys = new[]
        {
            new RedisKey("1"),
            new RedisKey("2"),
            new RedisKey("3")
        }.ToAsyncEnumerable();
        server2
            .KeysAsync(pattern: "some pattern")
            .Returns(keys);

        var result = await _sut.GetKeysAsync("some pattern");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(Error.New(exception)));

        server2
            .DidNotReceiveWithAnyArgs()
            .KeysAsync();
    }

    [Test]
    public async Task GetKeysAsync_WhenServersThrow_ShouldReturnFirstError()
    {
        var exception1 = new Exception("some message");
        var exception2 = new Exception("some message");

        var server1 = Substitute.For<IServer>();
        var server2 = Substitute.For<IServer>();

        var servers = new[]
        {
            server1,
            server2
        };

        _mockSourcesProvider
            .GetServers()
            .Returns(servers);

        server1
            .KeysAsync(pattern: "some pattern")
            .Throws(exception1);

        server2
            .KeysAsync(pattern: "some pattern")
            .Throws(exception2);

        var result = await _sut.GetKeysAsync("some pattern");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(Error.New(exception1)));

        server2
            .DidNotReceiveWithAnyArgs()
            .KeysAsync();
    }

    [Test]
    public async Task GetKeysAsync_WhenServersReturnKeys_ShouldMergeAndReturnThem()
    {
        var keys1 = new[]
        {
            new RedisKey("1.1"),
            new RedisKey("1.2"),
            new RedisKey("1.3")
        }.ToAsyncEnumerable();
        _mockServer1
            .KeysAsync(pattern: "some pattern")
            .Returns(keys1);
        var keys2 = new[]
        {
            new RedisKey("2.1"),
            new RedisKey("2.2")
        }.ToAsyncEnumerable();
        _mockServer2
            .KeysAsync(pattern: "some pattern")
            .Returns(keys2);

        var result = await _sut.GetKeysAsync("some pattern");

        var resultKeys = new[] { "1.1", "1.2", "1.3", "2.1", "2.2" };
        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.SequenceEqual(resultKeys).Should().BeTrue());
    }

    [Test]
    public async Task GetKeysAsync_WhenServersReturnKeysWithPrefix_ShouldCleanUpMergeAndReturnThem()
    {
        var keys1 = new[]
        {
            new RedisKey("1.1"),
            new RedisKey("1.2"),
            new RedisKey("1.3")
        }.ToAsyncEnumerable();

        _mockServer1
            .KeysAsync(pattern: "some pattern")
            .Returns(keys1);
        var keys2 = new[]
        {
            new RedisKey("2.1"),
            new RedisKey("2.2")
        }.ToAsyncEnumerable();
        _mockServer2
            .KeysAsync(pattern: "some pattern")
            .Returns(keys2);

        var result = await _sut.GetKeysAsync("some pattern");

        var resultKeys = new[] { "1.1", "1.2", "1.3", "2.1", "2.2" };
        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.SequenceEqual(resultKeys).Should().BeTrue());
    }
}
