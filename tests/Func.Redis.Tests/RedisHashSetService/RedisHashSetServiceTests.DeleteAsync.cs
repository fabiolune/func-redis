namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [TestCase("")]
    [TestCase(null)]
    [TestCase("something")]
    public async Task DeleteAsync_WhenDatabaseIsNull_ShouldReturnError(string prefix)
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = await _sut.DeleteAsync("key", "field");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [TestCase("", "key", "field")]
    [TestCase(":", "key", "field")]
    [TestCase("  :", "key", "field")]
    [TestCase("::", "key", "field")]
    [TestCase("prefix", "prefix:key", "field")]
    [TestCase("prefix:", "prefix:key", "field")]
    [TestCase("prefix::", "prefix:key", "field")]
    public async Task DeleteAsync_WhenDatabaseThrowsException_ShouldReturnError(string prefix, string key, string field)
    {
        var exception = new Exception("some message");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashDeleteAsync(key, field, Arg.Any<CommandFlags>())
            .Returns<bool>(_ => throw exception);

        var result = await _sut.DeleteAsync("key", "field");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Message.Should().Be("some message"));
        await _mockDb
            .Received(1)
            .HashDeleteAsync(key, field, Arg.Any<CommandFlags>());
    }

    [TestCase("", "key", "field", true)]
    [TestCase(":", "key", "field", true)]
    [TestCase(" :", "key", "field", true)]
    [TestCase("::", "key", "field", true)]
    [TestCase("prefix", "prefix:key", "field", true)]
    [TestCase("prefix:", "prefix:key", "field", true)]
    [TestCase("prefix::", "prefix:key", "field", true)]
    [TestCase("", "key", "field", false)]
    [TestCase(":", "key", "field", false)]
    [TestCase("::", "key", "field", false)]
    [TestCase("prefix", "prefix:key", "field", false)]
    [TestCase("prefix:", "prefix:key", "field", false)]
    [TestCase("prefix::", "prefix:key", "field", false)]
    public async Task DeleteAsync_WhenDatabaseReturnsValidBool_ShouldReturnRightWithSome(string prefix, string key, string field, bool returnValue)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashDeleteAsync(key, field, Arg.Any<CommandFlags>())
            .Returns(returnValue);

        var result = await _sut.DeleteAsync("key", "field");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.Should().Be(Unit.Default));
    }

    [TestCase("", "key", "field1", "field2")]
    [TestCase(":", "key", "field1", "field2")]
    [TestCase("  :", "key", "field1", "field2")]
    [TestCase("::", "key", "field1", "field2")]
    [TestCase("prefix", "prefix:key", "field1", "field2")]
    [TestCase("prefix:", "prefix:key", "field1", "field2")]
    [TestCase("prefix::", "prefix:key", "field1", "field2")]
    public async Task MultipleDeleteAsync_WhenDatabaseThrowsException_ShouldReturnError(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var _params = new[] { "field1", "field2" };
        var exception = new Exception("some message");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        _mockDb
            .HashDeleteAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns<long>(_ => throw exception);

        var result = await _sut.DeleteAsync("key", _params);

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .HashDeleteAsync((RedisKey)key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase("", "key", "field1", "field2", 0)]
    [TestCase(":", "key", "field1", "field2", 0)]
    [TestCase(" :", "key", "field1", "field2", 0)]
    [TestCase("::", "key", "field1", "field2", 0)]
    [TestCase("prefix", "prefix:key", "field1", "field2", 0)]
    [TestCase("prefix:", "prefix:key", "field1", "field2", 0)]
    [TestCase("prefix::", "prefix:key", "field1", "field2", 0)]
    [TestCase("", "key", "field1", "field2", 11)]
    [TestCase(":", "key", "field1", "field2", 11)]
    [TestCase(" :", "key", "field1", "field2", 11)]
    [TestCase("::", "key", "field1", "field2", 11)]
    [TestCase("prefix", "prefix:key", "field1", "field2", 11)]
    [TestCase("prefix:", "prefix:key", "field1", "field2", 11)]
    [TestCase("prefix::", "prefix:key", "field1", "field2", 11)]
    public async Task MultipleDeleteAsync_WhenDatabaseReturnsValidBool_ShouldReturnRightWithSome(string prefix, string key, string field1, string field2, long returnValue)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var requestParams = new[] { "field1", "field2" };

        _mockDb
            .HashDeleteAsync((RedisKey)key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(returnValue.AsTask());

        var result = await _sut.DeleteAsync("key", requestParams);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.Should().Be(Unit.Default));
    }
}