using NSubstitute.ExceptionExtensions;

namespace Func.Redis.Tests.RedisKeyService;
public partial class RedisKeyServiceTests
{
    [Test]
    public void GetKeys_WhenMultiplexerThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        _mockSourcesProvider
            .GetServers()
            .Throws(exception);

        var result = _sut.GetKeys("some pattern");
        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(Error.New(exception)));
    }

    [Test]
    public void GetKeys_WhenServerThrows_ShouldReturnError()
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
            .Keys(pattern: "some pattern")
            .Throws(exception);

        var keys = new[]
        {
            new RedisKey("1"),
            new RedisKey("2"),
            new RedisKey("3")
        };
        server2
            .Keys(pattern: "some pattern")
            .Returns(keys);

        var result = _sut.GetKeys("some pattern");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(Error.New(exception)));

        server2
            .DidNotReceiveWithAnyArgs()
            .Keys();
    }

    [Test]
    public void GetKeys_WhenServersThrow_ShouldReturnFirstError()
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
            .Keys(pattern: "some pattern")
            .Throws(exception1);

        server2
            .Keys(pattern: "some pattern")
            .Throws(exception2);

        var result = _sut.GetKeys("some pattern");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(Error.New(exception1)));

        server2
            .DidNotReceiveWithAnyArgs()
            .Keys();
    }

    [Test]
    public void GetKeys_WhenServersReturnKeys_ShouldMergeAndReturnThem()
    {
        var keys1 = new[]
        {
            new RedisKey("1.1"),
            new RedisKey("1.2"),
            new RedisKey("1.3")
        };
        _mockServer1
            .Keys(pattern: "some pattern")
            .Returns(keys1);
        var keys2 = new[]
        {
            new RedisKey("2.1"),
            new RedisKey("2.2")
        };
        _mockServer2
            .Keys(pattern: "some pattern")
            .Returns(keys2);

        var result = _sut.GetKeys("some pattern");

        var resultKeys = new[] { "1.1", "1.2", "1.3", "2.1", "2.2" };
        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.SequenceEqual(resultKeys).ShouldBeTrue());
    }

    [Test]
    public void GetKeys_WhenServersReturnKeysWithPrefix_ShouldCleanUpMergeAndReturnThem()
    {
        var keys1 = new[]
        {
            new RedisKey("1.1"),
            new RedisKey("1.2"),
            new RedisKey("1.3")
        };
        _mockServer1
            .Keys(pattern: "some pattern")
            .Returns(keys1);
        var keys2 = new[]
        {
            new RedisKey("2.1"),
            new RedisKey("2.2")
        };
        _mockServer2
            .Keys(pattern: "some pattern")
            .Returns(keys2);

        var result = _sut.GetKeys("some pattern");

        _mockServer1
            .Received(1)
            .Keys(pattern: "some pattern");
        _mockServer2
            .Received(1)
            .Keys(pattern: "some pattern");

        var resultKeys = new[] { "1.1", "1.2", "1.3", "2.1", "2.2" };

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.SequenceEqual(resultKeys).ShouldBeTrue());
    }
}
