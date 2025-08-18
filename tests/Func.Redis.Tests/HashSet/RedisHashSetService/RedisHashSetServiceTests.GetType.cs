namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    public record TestData2(int OtherId);

    [Test]
    public void MultiGetType_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.HashSet.RedisHashSetService(_mockProvider, _mockSerDes);

        var typeFields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.Get("key", typeFields);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public void MultiGetType_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var typefields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };

        var exception = new Exception("some message");

        _mockDb
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Get("key", typefields);

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));

        _mockDb
            .Received(1)
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase("")]
    [TestCase(null)]
    public void MultiGetType_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string content)
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)content, (RedisValue)content };
        var typefields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };

        _mockDb
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
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
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [Test]
    public void MultiGetType_WhenDatabaseReturnsNonJsonValue_ShouldReturnLeft()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { new RedisValue(@"{""wrong"": ""json"), new RedisValue(@"{""wrong"": ""json") };
        var typefields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };

        _mockDb
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
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
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [Test]
    public void MultiGetType_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { RedisValue.Null, RedisValue.Null };
        var typefields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };

        _mockDb
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
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
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [Test]
    public void MultiGetType_WhenSerDesReturnsProperData_ShouldReturnRightWithSome()
    {
        var type1 = typeof(TestData);
        var type2 = typeof(TestData2);

        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)"serialized 1", (RedisValue)"serialized 2" };
        var stringfields = new[] { (type1, "field1"), (type2, "field2") };

        _mockDb
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(c => contents);
        var obj1 = new TestData(1).ToOption<object>();
        var obj2 = new TestData2(2).ToOption<object>();
        _mockSerDes
            .Deserialize((RedisValue)"serialized 1", type1)
            .Returns(obj1);
        _mockSerDes
            .Deserialize((RedisValue)"serialized 2", type2)
            .Returns(obj2);

        var result = _sut.Get("key", stringfields);

        result
            .OnRight(e => e.Should().BeEquivalentTo([obj1, obj2]));
    }

    [Test]
    public async Task MultiGetAsyncType_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.HashSet.RedisHashSetService(_mockProvider, _mockSerDes);

        var typeFields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = await _sut.GetAsync("key", typeFields);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public async Task MultiGetAsyncType_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var typeFields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };

        var exception = new Exception("some message");

        _mockDb
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns<RedisValue[]>(_ => throw exception);

        var result = await _sut.GetAsync("key", typeFields);

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase("")]
    [TestCase(null)]
    public async Task MultiGetAsyncType_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string content)
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)content, (RedisValue)content };
        var typeFields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };

        _mockDb
             .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
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
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task MultiGetAsyncType_WhenDatabaseReturnsNonJsonValue_ShouldReturnLeft()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { new RedisValue(@"{""wrong"": ""json"), new RedisValue(@"{""wrong"": ""json") };

        var typeFields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };

        _mockDb
             .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
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
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task MultiGetAsyncType_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { RedisValue.Null, RedisValue.Null };
        var typeFields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };

        _mockDb
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
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
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task MultiGetAsyncType_WhenDatabaseReturnsValidJson_ShouldReturnRightWithSome()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)"serialized 1", (RedisValue)"serialized 2" };
        var typeFields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };

        _mockDb
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents.AsTask());
        _mockSerDes
            .Deserialize("serialized 1", typeof(TestData))
            .Returns(new TestData(1).ToOption<object>());
        _mockSerDes
            .Deserialize("serialized 2", typeof(TestData))
            .Returns(new TestData(2).ToOption<object>());

        var result = await _sut.GetAsync("key", typeFields);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.Should().HaveCount(2);
                e.Filter().Should().BeEquivalentTo([new TestData(1), new TestData(2)]);
            });
    }

    [Test]
    public async Task MultiGetAsyncType_WhenDatabaseReturnsValidJson_ShouldReturnRightWithNoneWhereMissingField()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)"serialized" };
        var typeFields = new[] { (typeof(TestData), "field1"), (typeof(TestData), "field2") };

        _mockDb
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents.AsTask());
        _mockSerDes
           .Deserialize("serialized", typeof(TestData))
           .Returns(new TestData(1).ToOption<object>());

        var result = await _sut.GetAsync("key", typeFields);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.Should().HaveCount(1);
                e.Filter().Should().BeEquivalentTo([new TestData(1)]);
            });
    }
}