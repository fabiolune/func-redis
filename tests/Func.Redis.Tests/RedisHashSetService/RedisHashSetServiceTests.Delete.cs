namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [TestCase("")]
    [TestCase(null)]
    [TestCase("something")]
    public void Delete_WhenDatabaseIsNull_ShouldReturnError(string prefix)
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.Delete("key", "field");

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
    public void Delete_WhenDatabaseThrowsException_ShouldReturnError(string prefix, string key, string field)
    {
        var exception = new Exception("some message");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashDelete(key, field, Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Delete("key", "field");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Message.Should().Be("some message"));
        _mockDb
            .Received(1)
            .HashDelete(key, field, Arg.Any<CommandFlags>());
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
    public void Delete_WhenDatabaseReturnsValidBool_ShouldReturnRightWithSome(string prefix, string key, string field, bool returnValue)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashDelete(key, field, Arg.Any<CommandFlags>())
            .Returns(returnValue);

        var result = _sut.Delete("key", "field");

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
    public void MultipleDelete_WhenDatabaseThrowsException_ShouldReturnError(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var parameters = new[] { "field1", "field2" };
        var exception = new Exception("some message");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        _mockDb
            .HashDelete(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Delete("key", parameters);

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        _mockDb
            .Received(1)
            .HashDelete((RedisKey)key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
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
    public void MultipleDelete_WhenDatabaseReturnsValidBool_ShouldReturnRightWithSome(string prefix, string key, string field1, string field2, long returnValue)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var _params = new[] { "field1", "field2" };

        _mockDb
            .HashDelete((RedisKey)key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(returnValue);

        var result = _sut.Delete("key", _params);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.Should().Be(Unit.Default));
    }
}