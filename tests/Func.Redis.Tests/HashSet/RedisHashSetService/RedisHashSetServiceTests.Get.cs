namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [Test]
    public void Get_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.HashSet.RedisHashSetService(_mockProvider, _mockSerDes);

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.Get<object>("key", "field");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(err => err.ShouldBe(Error.New(new NullReferenceException())));
    }

    [Test]
    public void Get_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .HashGet("key", "field", Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Get<object>("key", "field");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(err => err.Message.ShouldBe("some message"));
        _mockDb
            .Received(1)
            .HashGet("key", "field", Arg.Any<CommandFlags>());
    }

    [TestCase(null)]
    [TestCase("")]
    public void Get_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string content)
    {
        _mockDb
            .HashGet("key", "field", Arg.Any<CommandFlags>())
            .Returns(new RedisValue(content));

        var result = _sut.Get<object>("key", "field");

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e => e.IsNone.ShouldBeTrue());
        _mockDb
            .Received(1)
            .HashGet("key", "field", Arg.Any<CommandFlags>());
    }

    [Test]
    public void Get_WhenDatabaseReturnsNonJsonValue_ShouldReturnLeft()
    {
        var redisReturn = new RedisValue("serialized");
        _mockDb
            .HashGet("key", "field", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        var exception = new Exception("some message");
        _mockSerDes
            .Deserialize<object>(redisReturn)
            .Returns(_ => throw exception);

        var result = _sut.Get<object>("key", "field");

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.Message.ShouldBe("some message"));
        _mockDb
            .Received(1)
            .HashGet("key", "field", CommandFlags.None);
    }

    [Test]
    public void Get_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone()
    {
        _mockDb
            .HashGet("key", "field", Arg.Any<CommandFlags>())
            .Returns(RedisValue.Null);

        var result = _sut.Get<object>("key", "field");

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e => e.IsNone.ShouldBeTrue());
        _mockDb
            .Received(1)
            .HashGet("key", "field", Arg.Any<CommandFlags>());
    }

    [Test]
    public void Get_WhenDatabaseReturnsValidJson_ShouldReturnRightWithSome()
    {
        var redisReturn = new RedisValue("serialized");
        _mockDb
            .HashGet("key", "field", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new TestData(1).ToOption());

        var result = _sut.Get<TestData>("key", "field");

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e =>
            {
                e.IsSome.ShouldBeTrue();
                e.OnSome(d => d.ShouldBeEquivalentTo(new TestData(1)));
            });
        _mockDb
            .Received(1)
            .HashGet("key", "field", CommandFlags.None);
    }

    [Test]
    public void MultiGet_WhenDatabaseIsNull_ShouldReturnError()
    {
        var fields = new[] { "field1", "field2" };

        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.HashSet.RedisHashSetService(_mockProvider, _mockSerDes);

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.Get<object>("key", fields);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(err => err.ShouldBe(Error.New(new NullReferenceException())));
    }

    [Test]
    public void MultiGet_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var stringfields = new[] { "field1", "field2" };

        var exception = new Exception("some message");

        _mockDb
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Get<object>("key", stringfields);

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
        _mockDb
            .Received(1)
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase(null)]
    [TestCase("")]
    public void MultiGet_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string content)
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)content, (RedisValue)content };
        var stringfields = new[] { "field1", "field2" };

        _mockDb
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents);

        var result = _sut.Get<object>("key", stringfields);

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e =>
            {
                e.Length.ShouldBe(2);
                e.Filter().ShouldBeEmpty();
            });

        _mockDb
            .Received(1)
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [Test]
    public void MultiGet_WhenDatabaseReturnsNonJsonValue_ShouldReturnLeft()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { new RedisValue(@"{""wrong"": ""json"), new RedisValue(@"{""wrong"": ""json") };
        var stringfields = new[] { "field1", "field2" };

        _mockDb
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents);

        var result = _sut.Get<object>("key", stringfields);

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e =>
            {
                e.Length.ShouldBe(2);
                e.Filter().ShouldBeEmpty();
            });

        _mockDb
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [Test]
    public void MultiGet_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { RedisValue.Null, RedisValue.Null };
        var stringfields = new[] { "field1", "field2" };

        _mockDb
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents);

        var result = _sut.Get<object>("key", stringfields);

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e =>
            {
                e.Length.ShouldBe(2);
                e.Filter().ShouldBeEmpty();
            });
        _mockDb
            .Received(1)
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [Test]
    public void MultiGet_WhenDatabaseReturnsValidJson_ShouldReturnRightWithSome()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var contents = new[] { (RedisValue)"serialized 1", (RedisValue)"serialized 2" };
        var stringfields = new[] { "field1", "field2" };

        _mockDb
            .HashGet("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(contents);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized 1")
            .Returns(new TestData(1).ToOption());

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized 2")
            .Returns(new TestData(2).ToOption());

        var result = _sut.Get<TestData>("key", stringfields);

        result
            .OnRight(r =>
            {
                r.Length.ShouldBe(2);
                var somes = r.Filter();
                var expected = new[] { new TestData(1), new TestData(2) };
                somes.ShouldBe(expected);
            });
    }
}