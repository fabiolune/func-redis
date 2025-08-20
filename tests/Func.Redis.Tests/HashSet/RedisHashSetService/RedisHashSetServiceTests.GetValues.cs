namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [Test]
    public void GetValues_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.HashSet.RedisHashSetService(_mockProvider, _mockSerDes);

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.GetValues<object>("key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(err => err.ShouldBe(Error.New(new NullReferenceException())));
    }

    [Test]
    public void GetValues_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .HashValues("key", Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.GetValues<object>("key");

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
        _mockDb
            .Received(1)
            .HashValues("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public void GetValues_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone()
    {
        _mockDb
            .HashValues("key", Arg.Any<CommandFlags>())
            .Returns([]);

        var result = _sut.GetValues<object>("key");

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e => e.IsNone.ShouldBeTrue());
        _mockDb
            .Received(1)
            .HashValues("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public void GetValues_WheSerDesThrows_ShouldReturnLeft()
    {
        var redisReturn = new RedisValue[] { "serialized" };
        _mockDb
            .HashValues("key", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        var exception = new Exception("some message");
        _mockSerDes
            .Deserialize<object>(redisReturn)
            .Returns(_ => throw exception);

        var result = _sut.GetValues<object>("key");

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBe(Error.New(exception)));
        _mockDb
            .Received(1)
            .HashValues("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public void GetValues_WhenDatabaseReturnsValidJson_ShouldReturnRightWithSome()
    {
        var redisReturn = new RedisValue[] { "serialized" };
        _mockDb
            .HashValues("key", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new[] { new TestData(1) }.ToOption());

        var result = _sut.GetValues<TestData>("key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.OnSome(d => d.ShouldBe([new TestData(1)])));

        _mockDb
            .Received(1)
            .HashValues("key", CommandFlags.None);
    }

    [Test]
    public void GetValues_WhenDatabaseReturnsMoreValidJsonSerialization_ShouldReturnRightWithSome()
    {
        var redisReturn = new RedisValue[] { "serialized 1", "serialized 2" };
        _mockDb
            .HashValues("key", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new[] { new TestData(1), new TestData(2) }.ToOption());

        var result = _sut.GetValues<TestData>("key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.OnSome(d => d.ShouldBe([new TestData(1), new TestData(2)])));
        _mockDb
            .Received(1)
            .HashValues("key", CommandFlags.None);
    }
}