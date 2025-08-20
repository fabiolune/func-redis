namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [Test]
    public void GetAll_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.HashSet.RedisHashSetService(_mockProvider, _mockSerDes);

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.GetAll<object>("key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(err => err.ShouldBe(Error.New(new NullReferenceException())));
    }

    [Test]
    public void GetAll_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .HashGetAll("key", Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.GetAll<object>("key");

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
        _mockDb
            .Received(1)
            .HashGetAll("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public void GetAll_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone()
    {
        _mockDb
            .HashGetAll("key", Arg.Any<CommandFlags>())
            .Returns([]);

        var result = _sut.GetAll<object>("key");

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e => e.IsNone.ShouldBeTrue());
        _mockDb
            .Received(1)
            .HashGetAll("key", CommandFlags.None);
    }

    [Test]
    public void GetAll_WhenSerDesThrows_ShouldReturnLeft()
    {
        var redisReturn = new HashEntry[] { new("field", "serialized") };
        _mockDb
            .HashGetAll("key", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        var exception = new Exception("some message");
        _mockSerDes
            .Deserialize<object>(redisReturn)
            .Returns(_ => throw exception);

        var result = _sut.GetAll<object>("key");

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBe(Error.New(exception)));
        _mockDb
            .Received(1)
            .HashGetAll("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public void GetAll_WhenDatabaseReturnsValidJson_ShouldReturnRightWithSome()
    {
        var redisReturn = new HashEntry[] { new("field", "serialized") };
        _mockDb
            .HashGetAll("key", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new[] { ("field", new TestData(1)) }.ToOption());

        var result = _sut.GetAll<TestData>("key");

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e =>
            {
                e.IsSome.ShouldBeTrue();
                e.OnSome(d =>
                {
                    (string, TestData)[] expected = [("field", new TestData(1))];
                    d.ShouldBe(expected);
                });
            });
    }

    [Test]
    public void GetAll_WhenDatabaseReturnsMoreValidJsonSerialization_ShouldReturnRightWithSome()
    {
        var redisReturn = new HashEntry[] { new("key", "serialized 1"), new("key", "serialized 2") };
        _mockDb
            .HashGetAll("key", Arg.Any<CommandFlags>())
            .Returns(redisReturn);

        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new[] { ("key", new TestData(1)), ("key", new TestData(2)) }.ToOption());

        var result = _sut.GetAll<TestData>("key");

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e =>
            {
                e.IsSome.ShouldBeTrue();
                e.OnSome(d =>
                {
                    (string, TestData)[] expected = [("key", new TestData(1)), ("key", new TestData(2))];
                    d.ShouldBe(expected);
                });
            });

        _mockDb
            .Received(1)
            .HashGetAll("key", CommandFlags.None);
    }
}