namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [TestCase("")]
    [TestCase(null)]
    [TestCase("something")]
    public async Task GetValuesAsync_WhenDatabaseIsNull_ShouldReturnError(string prefix)
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        var result = await _sut.GetValuesAsync<object>("key");

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
    public async Task GetValuesAsync_WhenDatabaseThrowsException_ShouldReturnError(string prefix, string key)
    {
        var exception = new Exception("some message");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashValuesAsync(key, Arg.Any<CommandFlags>())
            .Returns<Task<RedisValue[]>>(_ => throw exception);

        var result = await _sut.GetValuesAsync<object>("key");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .HashValuesAsync(key, Arg.Any<CommandFlags>());
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("  :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("  :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public async Task GetValuesAsync_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string prefix, string key)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashValuesAsync(key, Arg.Any<CommandFlags>())
            .Returns([]);

        var result = await _sut.GetValuesAsync<object>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        await _mockDb
            .Received(1)
            .HashValuesAsync(key, Arg.Any<CommandFlags>());
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("  :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public async Task GetValuesAsync_WhenSerDesThrows_ShouldReturnLeft(string prefix, string key)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        var redisReturn = new RedisValue[] { "something" };
        _mockDb
            .HashValuesAsync(key, Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        var exception = new Exception("some message");
        _mockSerDes
            .Deserialize<object>(redisReturn)
            .Returns(_ => throw exception);

        var result = await _sut.GetValuesAsync<object>("key");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .HashValuesAsync(key, Arg.Any<CommandFlags>());
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase(" :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public async Task GetValuesAsync_WhenDatabaseReturnsValidData_ShouldReturnRightWithSome(string prefix, string key)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        var redisReturn = new RedisValue[] { "serialized" };
        _mockDb
            .HashValuesAsync(key, Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new[] { new TestData("some-id") }.ToOption());

        var result = await _sut.GetValuesAsync<TestData>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.OnSome(d => d.Should().BeEquivalentTo(new[] { new TestData("some-id") })));
    }
}