namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [Test]
    public void GetAll_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes);

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.GetAll<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public void GetAll_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .HashGetAll("key", Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.GetAll<object>("key");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
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

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
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

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().Be(Error.New(exception)));
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
            .Returns(new[] { ("field", new TestData("some-id")) }.ToOption());

        var result = _sut.GetAll<TestData>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.IsSome.Should().BeTrue();
                e.OnSome(d => d.Should().BeEquivalentTo([("field", new TestData("some-id"))]));
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
            .Returns(new[] { ("key", new TestData("some-id")), ("key", new TestData("some-id2")) }.ToOption());

        var result = _sut.GetAll<TestData>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.IsSome.Should().BeTrue();
                e.OnSome(d =>
                    d.Should().BeEquivalentTo(new[]
                        {
                        ("key", new TestData ("some-id")),
                        ("key", new TestData ("some-id2"))
                        }));
            });

        _mockDb
            .Received(1)
            .HashGetAll("key", CommandFlags.None);
    }
}