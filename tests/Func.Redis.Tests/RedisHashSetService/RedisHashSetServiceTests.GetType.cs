namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    public record TestData2(string OtherId);

    [TestCase("")]
    [TestCase(null)]
    [TestCase("something")]
    public void MultiGetType_WhenDatabaseIsNull_ShouldReturnError(string prefix)
    {

        var typeFields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };
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

        var result = _sut.Get("key", typeFields);

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
    public void MultiGetType_WhenDatabaseThrowsException_ShouldReturnError(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var typefields = new[] { (typeof(TestData), field1), (typeof(TestData), field2) };

        var exception = new Exception("some message");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Get("key", typefields);

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
    public void MultiGetType_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string prefix, string key, string field1, string field2, string content)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var contents = new[] { (RedisValue)content, (RedisValue)content };
        var typefields = new[] { (typeof(TestData), field1), (typeof(TestData), field2) };

        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents);

        var result = _sut.Get("key", typefields);

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
    public void MultiGetType_WhenDatabaseReturnsNonJsonValue_ShouldReturnLeft(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var contents = new[] { new RedisValue(@"{""wrong"": ""json"), new RedisValue(@"{""wrong"": ""json") };
        var typefields = new[] { (typeof(TestData), field1), (typeof(TestData), field2) };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents);

        var result = _sut.Get("key", typefields);

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
    public void MultiGetType_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var contents = new[] { RedisValue.Null, RedisValue.Null };
        var typefields = new[] { (typeof(TestData), field1), (typeof(TestData), field2) };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents);

        var result = _sut.Get("key", typefields);

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
    public void MultiGetType_WhenSerDesReturnsProperData_ShouldReturnRightWithSome(string prefix, string key)
    {
        var type1 = typeof(TestData);
        var type2 = typeof(TestData2);
        const string field1 = "field1";
        const string field2 = "field2";
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var contents = new[] { (RedisValue)"serialized 1", (RedisValue)"serialized 2" };
        var stringfields = new[] { (type1, field1), (type2, field2) };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        _mockDb
            .HashGet(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(c => contents);
        var obj1 = new TestData("par 1").ToOption<object>();
        var obj2 = new TestData2("par 2").ToOption<object>();
        _mockSerDes
            .Deserialize((RedisValue)"serialized 1", type1)
            .Returns(obj1);
        _mockSerDes
            .Deserialize((RedisValue)"serialized 2", type2)
            .Returns(obj2);

        var result = _sut.Get("key", stringfields);

        result
            .OnRight(e => e.Should().BeEquivalentTo(new[] { obj1, obj2 }));
    }

    [TestCase("")]
    [TestCase(null)]
    [TestCase("something")]
    public async Task MultiGetAsyncType_WhenDatabaseIsNull_ShouldReturnError(string prefix)
    {

        var typeFields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };
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

        var result = await _sut.GetAsync("key", typeFields);

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
    public async Task MultiGetAsyncType_WhenDatabaseThrowsException_ShouldReturnError(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var typeFields = new[] { (typeof(TestData), field1), (typeof(TestData), field2) };

        var exception = new Exception("some message");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns<RedisValue[]>(_ => throw exception);

        var result = await _sut.GetAsync("key", typeFields);

        result.IsLeft.Should().BeTrue();
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
    public async Task MultiGetAsyncType_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string prefix, string key, string field1, string field2, string content)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var contents = new[] { (RedisValue)content, (RedisValue)content };
        var typeFields = new[] { (typeof(TestData), field1), (typeof(TestData), field2) };

        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
             .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
             .Returns(contents);

        var result = await _sut.GetAsync("key", typeFields);

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
    public async Task MultiGetAsyncType_WhenDatabaseReturnsNonJsonValue_ShouldReturnLeft(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var contents = new[] { new RedisValue(@"{""wrong"": ""json"), new RedisValue(@"{""wrong"": ""json") };

        var typeFields = new[] { (typeof(TestData), field1), (typeof(TestData), field2) };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
             .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
             .Returns(contents.AsTask());
        
        var result = await _sut.GetAsync("key", typeFields);

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
    public async Task MultiGetAsyncType_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone(string prefix, string key, string field1, string field2)
    {
        var fields = new[] { (RedisValue)field1, (RedisValue)field2 };
        var contents = new[] { RedisValue.Null, RedisValue.Null };
        var typeFields = new[] { (typeof(TestData), field1), (typeof(TestData), field2) };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents.AsTask());

        var result = await _sut.GetAsync("key", typeFields);

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
    public async Task MultiGetAsyncType_WhenDatabaseReturnsValidJson_ShouldReturnRightWithSome(string prefix, string key)
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)"serialized 1", (RedisValue)"serialized 2" };
        var typeFields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents.AsTask());
        _mockSerDes
            .Deserialize("serialized 1", typeof(TestData))
            .Returns(new TestData("id 1").ToOption<object>());
        _mockSerDes
            .Deserialize("serialized 2", typeof(TestData))
            .Returns(new TestData("id 2").ToOption<object>());

        var result = await _sut.GetAsync("key", typeFields);

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
    public async Task MultiGetAsyncType_WhenDatabaseReturnsValidJson_ShouldReturnRightWithNoneWhereMissingField(string prefix, string key)
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)"serialized" };
        var typeFields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashGetAsync(key, Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents.AsTask());
        _mockSerDes
           .Deserialize("serialized", typeof(TestData))
           .Returns(new TestData("id 1").ToOption<object>());

        var result = await _sut.GetAsync("key", typeFields);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.Should().HaveCount(1);
                e.Filter().Should().BeEquivalentTo([new TestData("id 1")]);
            });
    }
}