using static Func.Redis.Tests.TestDataElements;

namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [Test]
    public async Task GetAsync_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.HashSet.RedisHashSetService(_mockProvider, _mockSerDes);

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = await _sut.GetAsync<object>("key", "field");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public async Task GetAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .HashGetAsync("key", "field", Arg.Any<CommandFlags>())
            .Returns<RedisValue>(_ => throw exception);

        var result = await _sut.GetAsync<object>("key", "field");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .HashGetAsync("key", "field", Arg.Any<CommandFlags>());
    }

    [TestCase("")]
    [TestCase(null)]
    public async Task GetAsync_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string content)
    {
        _mockDb
            .HashGetAsync("key", "field", Arg.Any<CommandFlags>())
            .Returns(new RedisValue(content));

        var result = await _sut.GetAsync<object>("key", "field");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        await _mockDb
            .Received(1)
            .HashGetAsync("key", "field", Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task GetAsync_WhenSerDesThrows_ShouldReturnLeft()
    {
        var redisReturn = new RedisValue("serialized");
        _mockDb
            .HashGetAsync("key", "field", Arg.Any<CommandFlags>())
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
            .HashGetAsync("key", "field", Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task GetAsync_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone()
    {
        _mockDb
            .HashGetAsync("key", "field", Arg.Any<CommandFlags>())
            .Returns(RedisValue.Null);

        var result = await _sut.GetAsync<object>("key", "field");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        await _mockDb
            .Received(1)
            .HashGetAsync("key", "field", Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task GetAsync_WhenDatabaseReturnsValidJson_ShouldReturnRightWithSome()
    {
        var redisReturn = new RedisValue("serialized");
        _mockDb
            .HashGetAsync("key", "field", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new TestData(1).ToOption());

        var result = await _sut.GetAsync<TestData>("key", "field");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.IsSome.Should().BeTrue();
                e.OnSome(d => d.Should().BeEquivalentTo(new TestData(1)));
            });
    }

    [Test]
    public async Task MultiGetAsync_WhenDatabaseIsNull_ShouldReturnError()
    {
        var fields = new[] { "field1", "field2" };
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.HashSet.RedisHashSetService(_mockProvider, _mockSerDes);

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = await _sut.GetAsync<object>("key", fields);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public async Task MultiGetAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var stringfields = new[] { "field1", "field2" };

        var exception = new Exception("some message");

        _mockDb
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns<RedisValue[]>(_ => throw exception);

        var result = await _sut.GetAsync<object>("key", stringfields);

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase("")]
    [TestCase(null)]
    public async Task MultiGetAsync_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string content)
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)content, (RedisValue)content };
        var stringfields = new[] { "field1", "field2" };

        _mockDb
             .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
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
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task MultiGetAsync_WhenDatabaseReturnsNonJsonValue_ShouldReturnLeft()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { new RedisValue(@"{""wrong"": ""json"), new RedisValue(@"{""wrong"": ""json") };
        var stringfields = new[] { "field1", "field2" };

        _mockDb
             .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
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
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task MultiGetAsync_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { RedisValue.Null, RedisValue.Null };
        var stringfields = new[] { "field1", "field2" };

        _mockDb
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
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
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task MultiGetAsync_WhenDatabaseReturnsValidData_ShouldReturnRightWithSome()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)"serialized 1", (RedisValue)"serialized 2" };
        var stringfields = new[] { "field1", "field2" };

        _mockDb
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents.AsTask());
        _mockSerDes
            .Deserialize<TestData>("serialized 1")
            .Returns(new TestData(1).ToOption());
        _mockSerDes
            .Deserialize<TestData>("serialized 2")
            .Returns(new TestData(2).ToOption());

        var result = await _sut.GetAsync<TestData>("key", stringfields);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.Should().HaveCount(2);
                e.Filter().Should().BeEquivalentTo([new TestData(1), new TestData(2)]);
            });
    }

    [Test]
    public async Task MultiGetAsync_WhenDatabaseReturnsValidJson_ShouldReturnRightWithNoneWhereMissingField()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)"serialized" };
        var stringfields = new[] { "field1", "field2" };

        _mockDb
            .HashGetAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents.AsTask());
        _mockSerDes
           .Deserialize<TestData>("serialized")
           .Returns(new TestData(1).ToOption());

        var result = await _sut.GetAsync<TestData>("key", stringfields);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.Should().HaveCount(1);
                e.Filter().Should().BeEquivalentTo([new TestData(1)]);
            });
    }
}