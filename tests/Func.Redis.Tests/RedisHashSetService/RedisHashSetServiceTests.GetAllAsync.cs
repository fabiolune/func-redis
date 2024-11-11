namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [Test]
    public async Task GetAllAsync_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new HashSet.RedisHashSetService(_mockProvider, _mockSerDes);

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = await _sut.GetAllAsync<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public async Task GetAllAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .HashGetAllAsync("key", Arg.Any<CommandFlags>())
            .Returns<HashEntry[]>(_ => throw exception);

        var result = await _sut.GetAllAsync<object>("key");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .HashGetAllAsync("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task GetAllAsync_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone()
    {
        _mockDb
            .HashGetAllAsync("key", Arg.Any<CommandFlags>())
            .Returns([]);

        var result = await _sut.GetAllAsync<object>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        await _mockDb
            .Received(1)
            .HashGetAllAsync("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task GetAllAsync_WhenDatabaseReturnsValidData_ShouldReturnRightWithSome()
    {
        var redisReturn = new HashEntry[] { new("key", "serialized") };
        _mockDb
            .HashGetAllAsync("key", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new[] { ("key", new TestData("some-id")) }.ToOption());

        var result = await _sut.GetAllAsync<TestData>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.IsSome.Should().BeTrue();
                e.OnSome(d => d.Should().BeEquivalentTo([("key", new TestData("some-id"))]));
            });
        await _mockDb
            .Received(1)
            .HashGetAllAsync("key", CommandFlags.None);
    }
}