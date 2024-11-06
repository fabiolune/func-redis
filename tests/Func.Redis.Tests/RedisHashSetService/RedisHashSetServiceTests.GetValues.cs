namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [TestCase("")]
    [TestCase(null)]
    [TestCase("something")]
    public void GetValues_WhenDatabaseIsNull_ShouldReturnError(string prefix)
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.GetValues<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("  :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public void GetValues_WhenDatabaseThrowsException_ShouldReturnError(string prefix, string key)
    {
        var exception = new Exception("some message");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashValues(key, Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.GetValues<object>("key");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        _mockDb
            .Received(1)
            .HashValues(key, Arg.Any<CommandFlags>());
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("  :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public void GetValues_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string prefix, string key)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashValues(key, Arg.Any<CommandFlags>())
            .Returns([]);

        var result = _sut.GetValues<object>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        _mockDb
            .Received(1)
            .HashValues(key, Arg.Any<CommandFlags>());
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("  :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public void GetValues_WheSerDesThrows_ShouldReturnLeft(string prefix, string key)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        var redisReturn = new RedisValue[] { "serialized" };
        _mockDb
            .HashValues(key, Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        var exception = new Exception("some message");
        _mockSerDes
            .Deserialize<object>(redisReturn)
            .Returns(_ => throw exception);

        var result = _sut.GetValues<object>("key");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().Be(Error.New(exception)));
        _mockDb
            .Received(1)
            .HashValues(key, Arg.Any<CommandFlags>());
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("  :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public void GetValues_WhenDatabaseReturnsValidJson_ShouldReturnRightWithSome(string prefix, string key)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        var redisReturn = new RedisValue[] { "serialized" };
        _mockDb
            .HashValues(key, Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new[] { new TestData("some-id") }.ToOption());

        var result = _sut.GetValues<TestData>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
                e.OnSome(d =>
                    d.Should().BeEquivalentTo(new[] { new TestData("some-id") })));

        _mockDb
            .Received(1)
            .HashValues(key, CommandFlags.None);
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("  :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public void GetValues_WhenDatabaseReturnsMoreValidJsonSerialization_ShouldReturnRightWithSome(string prefix, string key)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        var redisReturn = new RedisValue[] { "serialized 1", "serialized 2" };
        _mockDb
            .HashValues(key, Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new[] { new TestData("some-id"), new TestData("some-id2") }.ToOption());

        var result = _sut.GetValues<TestData>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
                e.OnSome(d =>
                    d.Should().BeEquivalentTo([new TestData("some-id"), new TestData("some-id2")])));
        _mockDb
            .Received(1)
            .HashValues(key, CommandFlags.None);
    }
}