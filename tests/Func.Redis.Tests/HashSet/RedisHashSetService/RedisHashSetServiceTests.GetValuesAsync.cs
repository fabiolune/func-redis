namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [Test]
    public async Task GetValuesAsync_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.HashSet.RedisHashSetService(_mockProvider, _mockSerDes);

        var result = await _sut.GetValuesAsync<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public async Task GetValuesAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .HashValuesAsync("key", Arg.Any<CommandFlags>())
            .Returns<Task<RedisValue[]>>(_ => throw exception);

        var result = await _sut.GetValuesAsync<object>("key");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .HashValuesAsync("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task GetValuesAsync_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone()
    {
        _mockDb
            .HashValuesAsync("key", Arg.Any<CommandFlags>())
            .Returns([]);

        var result = await _sut.GetValuesAsync<object>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        await _mockDb
            .Received(1)
            .HashValuesAsync("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task GetValuesAsync_WhenSerDesThrows_ShouldReturnLeft()
    {
        var redisReturn = new RedisValue[] { "something" };
        _mockDb
            .HashValuesAsync("key", Arg.Any<CommandFlags>())
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
            .HashValuesAsync("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task GetValuesAsync_WhenDatabaseReturnsValidData_ShouldReturnRightWithSome()
    {
        var redisReturn = new RedisValue[] { "serialized" };
        _mockDb
            .HashValuesAsync("key", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new[] { new TestData(1) }.ToOption());

        var result = await _sut.GetValuesAsync<TestData>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.OnSome(d => d.Should().BeEquivalentTo([new TestData(1)])));
    }
}