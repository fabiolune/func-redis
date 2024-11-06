namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [TestCase("")]
    [TestCase(null)]
    [TestCase("something")]
    public async Task GetAsync_WhenDatabaseIsNull_ShouldReturnError(string prefix)
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = await _sut.GetAsync<object>("key", "field");

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
    public async Task GetAsync_WhenDatabaseThrowsException_ShouldReturnError(string prefix, string key, string field)
    {
        var exception = new Exception("some message");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGetAsync(key, field, Arg.Any<CommandFlags>())
            .Returns<RedisValue>(_ => throw exception);

        var result = await _sut.GetAsync<object>("key", "field");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .HashGetAsync(key, field, Arg.Any<CommandFlags>());
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
    public async Task GetAsync_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string prefix, string key, string field, string content)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        _mockDb
            .HashGetAsync(key, field, Arg.Any<CommandFlags>())
            .Returns(new RedisValue(content));

        var result = await _sut.GetAsync<object>("key", "field");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        await _mockDb
            .Received(1)
            .HashGetAsync(key, field, Arg.Any<CommandFlags>());
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("  :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public async Task GetAsync_WhenSerDesThrows_ShouldReturnLeft(string prefix, string key)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        var redisReturn = new RedisValue("serialized");
        _mockDb
            .HashGetAsync(key, "field", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        var exception = new Exception("some message");
        _mockSerDes
            .Deserialize<object>(redisReturn)
            .Returns(_ => throw exception);

        var result = await _sut.GetAsync<object>("key", "field");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().Be(Error.New(exception)));
        await _mockDb
            .Received(1)
            .HashGetAsync(key, "field", Arg.Any<CommandFlags>());
    }

    [TestCase("", "key", "field")]
    [TestCase(":", "key", "field")]
    [TestCase("::", "key", "field")]
    [TestCase(" :", "key", "field")]
    [TestCase("  :", "key", "field")]
    [TestCase("prefix", "prefix:key", "field")]
    [TestCase("prefix:", "prefix:key", "field")]
    [TestCase("prefix::", "prefix:key", "field")]
    public async Task GetAsync_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone(string prefix, string key, string field)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGetAsync(key, field, Arg.Any<CommandFlags>())
            .Returns(RedisValue.Null);

        var result = await _sut.GetAsync<object>("key", "field");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        await _mockDb
            .Received(1)
            .HashGetAsync(key, field, Arg.Any<CommandFlags>());
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase(" :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public async Task GetAsync_WhenDatabaseReturnsValidJson_ShouldReturnRightWithSome(string prefix, string key)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        var redisReturn = new RedisValue("serialized");
        _mockDb
            .HashGetAsync(key, "field", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new TestData("some-id").ToOption());

        var result = await _sut.GetAsync<TestData>("key", "field");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.IsSome.Should().BeTrue();
                e.OnSome(d => d.Should().BeEquivalentTo(new TestData("some-id")));
            });
    }

    [TestCase("")]
    [TestCase(null)]
    [TestCase("something")]
    public async Task MultiGetAsync_WhenDatabaseIsNull_ShouldReturnError(string prefix)
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

        var result = await _sut.GetAsync<object>("key", fields);

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
    public async Task MultiGetAsync_WhenDatabaseThrowsException_ShouldReturnError(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var stringfields = new[] { field1, field2 };

        var exception = new Exception("some message");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns<RedisValue[]>(_ => throw exception);

        var result = await _sut.GetAsync<object>("key", stringfields);

        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
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
    public async Task MultiGetAsync_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string prefix, string key, string field1, string field2, string content)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var contents = new[] { (RedisValue)content, (RedisValue)content };
        var stringfields = new[] { field1, field2 };

        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
             .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
             .Returns(contents);

        var result = await _sut.GetAsync<object>("key", stringfields);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.Should().HaveCount(2);
                e.Filter().Should().BeEmpty();
            });

        await _mockDb
            .Received(1)
            .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase("", "key", "field11", "field2")]
    [TestCase(":", "key", "field1", "field2")]
    [TestCase("  :", "key", "field1", "field2")]
    [TestCase("::", "key", "field1", "field2")]
    [TestCase("prefix", "prefix:key", "field1", "field2")]
    [TestCase("prefix:", "prefix:key", "field1", "field2")]
    [TestCase("prefix::", "prefix:key", "field1", "field2")]
    public async Task MultiGetAsync_WhenDatabaseReturnsNonJsonValue_ShouldReturnLeft(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var contents = new[] { new RedisValue(@"{""wrong"": ""json"), new RedisValue(@"{""wrong"": ""json") };
        var stringfields = new[] { field1, field2 };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
             .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
             .Returns(contents.AsTask());
        var result = await _sut.GetAsync<object>("key", stringfields);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.Should().HaveCount(2);
                e.Filter().Should().BeEmpty();
            });

        await _mockDb
            .Received(1)
            .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase("", "key", "field11", "field2")]
    [TestCase(":", "key", "field11", "field2")]
    [TestCase("::", "key", "field11", "field2")]
    [TestCase(" :", "key", "field11", "field2")]
    [TestCase("  :", "key", "field11", "field2")]
    [TestCase("prefix", "prefix:key", "field11", "field2")]
    [TestCase("prefix:", "prefix:key", "field11", "field2")]
    [TestCase("prefix::", "prefix:key", "field11", "field2")]
    public async Task MultiGetAsync_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var contents = new[] { RedisValue.Null, RedisValue.Null };
        var stringfields = new[] { field1, field2 };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents.AsTask());

        var result = await _sut.GetAsync<object>("key", stringfields);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.Should().HaveCount(2);
                e.Filter().Should().BeEmpty();
            });
        await _mockDb
            .Received(1)
            .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase(" :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public async Task MultiGetAsync_WhenDatabaseReturnsValidData_ShouldReturnRightWithSome(string prefix, string key)
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)"serialized 1", (RedisValue)"serialized 2" };
        var stringfields = new[] { "field1", "field2" };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents.AsTask());
        _mockSerDes
            .Deserialize<TestData>("serialized 1")
            .Returns(new TestData("id 1").ToOption());
        _mockSerDes
            .Deserialize<TestData>("serialized 2")
            .Returns(new TestData("id 2").ToOption());

        var result = await _sut.GetAsync<TestData>("key", stringfields);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.Should().HaveCount(2);
                e.Filter().Should().BeEquivalentTo([new TestData("id 1"), new TestData("id 2")]);
            });
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase(" :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public async Task MultiGetAsync_WhenDatabaseReturnsValidJson_ShouldReturnRightWithNoneWhereMissingField(string prefix, string key)
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)"serialized" };
        var stringfields = new[] { "field1", "field2" };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents.AsTask());
        _mockSerDes
           .Deserialize<TestData>("serialized")
           .Returns(new TestData("id 1").ToOption());

        var result = await _sut.GetAsync<TestData>("key", stringfields);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.Should().HaveCount(1);
                e.Filter().Should().BeEquivalentTo([new TestData("id 1")]);
            });
    }
}