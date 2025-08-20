namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [Test]
    public async Task GetAllAsync_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.HashSet.RedisHashSetService(_mockProvider, _mockSerDes);

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = await _sut.GetAllAsync<object>("key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(err => err.ShouldBe(Error.New(new NullReferenceException())));
    }

    [Test]
    public async Task GetAllAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .HashGetAllAsync("key", Arg.Any<CommandFlags>())
            .Returns<HashEntry[]>(_ => throw exception);

        var result = await _sut.GetAllAsync<object>("key");

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
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

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e => e.IsNone.ShouldBeTrue());
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
            .Returns(new[] { ("key", new TestData(1)) }.ToOption());

        var result = await _sut.GetAllAsync<TestData>("key");

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e =>
            {
                e.IsSome.ShouldBeTrue();
                e.OnSome(d => d.ShouldBe([("key", new TestData(1))]));
            });
        await _mockDb
            .Received(1)
            .HashGetAllAsync("key", CommandFlags.None);
    }
}