namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [TestCase("")]
    [TestCase(null)]
    [TestCase("something")]
    public void Get_WhenDatabaseIsNull_ShouldReturnError(string prefix)
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.Get<object>("key", "field");

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
    public void Get_WhenDatabaseThrowsException_ShouldReturnError(string prefix, string key, string field)
    {
        var exception = new Exception("some message");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGet(key, field, Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Get<object>("key", "field");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Message.Should().Be("some message"));
        _mockDb
            .Received(1)
            .HashGet(key, field, Arg.Any<CommandFlags>());
    }

    [TestCase("", "key", "field", "")]
    [TestCase(":", "key", "field", "")]
    [TestCase("  :", "key", "field", "")]
    [TestCase("::", "key", "field", "")]
    [TestCase("prefix", "prefix:key", "field", "")]
    [TestCase("prefix:", "prefix:key", "field", "")]
    [TestCase("prefix::", "prefix:key", "field", "")]
    [TestCase("", "key", "field", null)]
    [TestCase(":", "key", "field", null)]
    [TestCase("  :", "key", "field", null)]
    [TestCase("::", "key", "field", null)]
    [TestCase("prefix", "prefix:key", "field", null)]
    [TestCase("prefix:", "prefix:key", "field", null)]
    [TestCase("prefix::", "prefix:key", "field", null)]
    public void Get_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string prefix, string key, string field, string content)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGet(key, field, Arg.Any<CommandFlags>())
            .Returns(new RedisValue(content));

        var result = _sut.Get<object>("key", "field");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        _mockDb
            .Received(1)
            .HashGet(key, field, Arg.Any<CommandFlags>());
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("  :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public void Get_WhenDatabaseReturnsNonJsonValue_ShouldReturnLeft(string prefix, string key)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        var redisReturn = new RedisValue("serialized");
        _mockDb
            .HashGet(key, "field", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        var exception = new Exception("some message");
        _mockSerDes
            .Deserialize<object>(redisReturn)
            .Returns(_ => throw exception);

        var result = _sut.Get<object>("key", "field");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Message.Should().Be("some message"));
        _mockDb
            .Received(1)
            .HashGet(key, "field", CommandFlags.None);
    }

    [TestCase("", "key", "field")]
    [TestCase(":", "key", "field")]
    [TestCase("::", "key", "field")]
    [TestCase(" :", "key", "field")]
    [TestCase("  :", "key", "field")]
    [TestCase("prefix", "prefix:key", "field")]
    [TestCase("prefix:", "prefix:key", "field")]
    [TestCase("prefix::", "prefix:key", "field")]
    public void Get_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone(string prefix, string key, string field)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGet(key, field, Arg.Any<CommandFlags>())
            .Returns(RedisValue.Null);

        var result = _sut.Get<object>("key", "field");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        _mockDb
            .Received(1)
            .HashGet(key, field, Arg.Any<CommandFlags>());
    }

    [TestCase(" :", "key", "field")]
    [TestCase("", "key", "field")]
    [TestCase(":", "key", "field")]
    [TestCase("::", "key", "field")]
    [TestCase("prefix", "prefix:key", "field")]
    [TestCase("prefix:", "prefix:key", "field")]
    [TestCase("prefix::", "prefix:key", "field")]
    public void Get_WhenDatabaseReturnsValidJson_ShouldReturnRightWithSome(string prefix, string key, string field)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        var redisReturn = new RedisValue("serialized");
        _mockDb
            .HashGet(key, field, Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new TestData("some-id").ToOption());

        var result = _sut.Get<TestData>("key", "field");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.IsSome.Should().BeTrue();
                e.OnSome(d => d.Should().BeEquivalentTo(new TestData("some-id")));
            });
        _mockDb
            .Received(1)
            .HashGet(key, field, CommandFlags.None);
    }

    [TestCase("")]
    [TestCase(null)]
    [TestCase("something")]
    public void MultiGet_WhenDatabaseIsNull_ShouldReturnError(string prefix)
    {

        var fields = new[] { "field1", "field2" };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.Get<object>("key", fields);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [TestCase("", "key", "field1", "field2")]
    [TestCase(":", "key", "field1", "field2")]
    [TestCase("  :", "key", "field1", "field2")]
    [TestCase("::", "key", "field1", "field2")]
    [TestCase("prefix", "prefix:key", "field1", "field2")]
    [TestCase("prefix:", "prefix:key", "field1", "field2")]
    [TestCase("prefix::", "prefix:key", "field1", "field2")]
    public void MultiGet_WhenDatabaseThrowsException_ShouldReturnError(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var stringfields = new[] { field1, field2 };

        var exception = new Exception("some message");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Get<object>("key", stringfields);

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        _mockDb
            .Received(1)
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase("", "key", "field1", "field2", "")]
    [TestCase(":", "key", "field1", "field2", "")]
    [TestCase("  :", "key", "field1", "field2", "")]
    [TestCase("::", "key", "field1", "field2", "")]
    [TestCase("prefix", "prefix:key", "field1", "field2", "")]
    [TestCase("prefix:", "prefix:key", "field1", "field2", "")]
    [TestCase("prefix::", "prefix:key", "field1", "field2", "")]
    [TestCase("", "key", "field1", "field2", null)]
    [TestCase(":", "key", "field1", "field2", null)]
    [TestCase("  :", "key", "field1", "field2", null)]
    [TestCase("::", "key", "field1", "field2", null)]
    [TestCase("prefix", "prefix:key", "field1", "field2", null)]
    [TestCase("prefix:", "prefix:key", "field1", "field2", null)]
    [TestCase("prefix::", "prefix:key", "field1", "field2", null)]
    public void MultiGet_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string prefix, string key, string field1, string field2, string content)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var contents = new[] { (RedisValue)content, (RedisValue)content };
        var stringfields = new[] { field1, field2 };

        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents);

        var result = _sut.Get<object>("key", stringfields);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.Should().HaveCount(2);
                e.Filter().Should().BeEmpty();
            });

        _mockDb
            .Received(1)
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase("", "key", "field11", "field2")]
    [TestCase(":", "key", "field1", "field2")]
    [TestCase("  :", "key", "field1", "field2")]
    [TestCase("::", "key", "field1", "field2")]
    [TestCase("prefix", "prefix:key", "field1", "field2")]
    [TestCase("prefix:", "prefix:key", "field1", "field2")]
    [TestCase("prefix::", "prefix:key", "field1", "field2")]
    public void MultiGet_WhenDatabaseReturnsNonJsonValue_ShouldReturnLeft(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var contents = new[] { new RedisValue(@"{""wrong"": ""json"), new RedisValue(@"{""wrong"": ""json") };
        var stringfields = new[] { field1, field2 };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents);

        var result = _sut.Get<object>("key", stringfields);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.Should().HaveCount(2);
                e.Filter().Should().BeEmpty();
            });

        _mockDb
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase("", "key", "field11", "field2")]
    [TestCase(":", "key", "field11", "field2")]
    [TestCase("::", "key", "field11", "field2")]
    [TestCase(" :", "key", "field11", "field2")]
    [TestCase("  :", "key", "field11", "field2")]
    [TestCase("prefix", "prefix:key", "field11", "field2")]
    [TestCase("prefix:", "prefix:key", "field11", "field2")]
    [TestCase("prefix::", "prefix:key", "field11", "field2")]
    public void MultiGet_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var contents = new[] { RedisValue.Null, RedisValue.Null };
        var stringfields = new[] { field1, field2 };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents);

        var result = _sut.Get<object>("key", stringfields);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.Should().HaveCount(2);
                e.Filter().Should().BeEmpty();
            });
        _mockDb
            .Received(1)
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase(" :", "key")]
    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public void MultiGet_WhenDatabaseReturnsValidJson_ShouldReturnRightWithSome(string prefix, string key)
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)"serialized 1", (RedisValue)"serialized 2" };
        var stringfields = new[] { "field1", "field2" };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized 1")
            .Returns(new TestData("id1").ToOption());

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized 2")
            .Returns(new TestData("id2").ToOption());

        var result = _sut.Get<TestData>("key", stringfields);

        result
            .OnRight(r =>
            {
                r.Should().HaveCount(2);
                var somes = r.Filter();
                somes.Should().HaveCount(2);
                somes.Should().BeEquivalentTo(new[] { new TestData("id1"), new TestData ("id2") });
            });
    }
}