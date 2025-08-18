namespace Func.Redis.Tests.RedisKeyService;

public partial class RedisKeyServiceTests
{
    [Test]
    public void Get_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockSourcesProvider.GetDatabase().Returns(null as IDatabase);

        _mockSourcesProvider
            .GetDatabase()
            .Returns(null as IDatabase);
        _sut = new Redis.Key.RedisKeyService(_mockSourcesProvider, _mockSerDes);

        var result = _sut.Get<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public void MultipleGet_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockSourcesProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.Key.RedisKeyService(_mockSourcesProvider, _mockSerDes);

        _mockSourcesProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.Get<object>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public void Get_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .StringGet("key", Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Get<object>("key");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        _mockDb
            .Received(1)
            .StringGet("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public void MultipleGet_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        _mockDb
            .StringGet(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Get<object>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        _mockDb
            .Received(1)
            .StringGet(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [TestCase(null)]
    [TestCase("")]
    public void Get_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string content)
    {
        _mockDb
            .StringGet("key", Arg.Any<CommandFlags>())
            .Returns(new RedisValue(content));

        var result = _sut.Get<object>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        _mockDb
            .Received(1)
            .StringGet("key", Arg.Any<CommandFlags>());
    }

    [TestCase("", "")]
    [TestCase("", null)]
    [TestCase(null, null)]
    public void MultipleGet_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string content1, string content2)
    {
        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        _mockDb
            .StringGet(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns([new RedisValue(content1), new RedisValue(content2)]);

        var result = _sut.Get<object>("key1", "key2");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.Where(o => o.IsSome).Should().HaveCount(0));
        _mockDb
            .Received(1)
            .StringGet(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [Test]
    public void Get_WhenSerdesThrowsException_ShouldReturnLeft()
    {
        var redisReturn = new RedisValue("something");
        _mockDb
            .StringGet("key", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        var exception = new Exception("some message");
        _mockSerDes
            .Deserialize<object>(redisReturn)
            .Returns(_ => throw exception);

        var result = _sut.Get<object>("key");

        _mockSerDes
            .Received(1)
            .Deserialize<object>(redisReturn);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Message.Should().Be("some message"));
        _mockDb
            .Received(1)
            .StringGet("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public void MultipleGet_WhenSerDesFailsOnMultipleValues_ShouldReturnLeft()
    {
        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        var redisReturn1 = new RedisValue("first");
        var redisReturn2 = new RedisValue("second");
        _mockDb
            .StringGet(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns([redisReturn1, redisReturn2]);

        var exception1 = new Exception("first message");
        var exception2 = new Exception("second message");

        _mockSerDes
            .Deserialize<TestData>(redisReturn1)
            .Returns(_ => throw exception1);
        _mockSerDes
            .Deserialize<TestData>(redisReturn2)
            .Returns(_ => throw exception2);

        var result = _sut.Get<TestData>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Message.Should().Be("first message"));
        _mockDb
            .Received(1)
            .StringGet(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [Test]
    public void MultipleGet_WhenSerDesFailsOnFirstValue_ShouldReturnLeft()
    {
        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        var redisReturn1 = new RedisValue("first");
        var redisReturn2 = new RedisValue("second");
        _mockDb
            .StringGet(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns([redisReturn1, redisReturn2]);

        var exception1 = new Exception("first message");

        _mockSerDes
            .Deserialize<TestData>(redisReturn1)
            .Returns(_ => throw exception1);
        _mockSerDes
            .Deserialize<TestData>(redisReturn2)
            .Returns(_ => new TestData(1).ToOption());

        var result = _sut.Get<TestData>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Message.Should().Be("first message"));
        _mockDb
            .Received(1)
            .StringGet(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [Test]
    public void MultipleGet_WhenSerDesFailsOnAnotherValue_ShouldReturnLeft()
    {
        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        var redisReturn1 = new RedisValue("first");
        var redisReturn2 = new RedisValue("second");
        _mockDb
            .StringGet(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns([redisReturn1, redisReturn2]);

        var exception = new Exception("second message");

        _mockSerDes
            .Deserialize<TestData>(redisReturn1)
            .Returns(Option<TestData>.Some(new TestData(1)));
        _mockSerDes
            .Deserialize<TestData>(redisReturn2)
            .Returns(_ => throw exception);

        var result = _sut.Get<TestData>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Message.Should().Be("second message"));
        _mockDb
            .Received(1)
            .StringGet(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [Test]
    public void Get_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone()
    {
        _mockDb
            .StringGet("key", Arg.Any<CommandFlags>())
            .Returns(RedisValue.Null);

        var result = _sut.Get<object>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        _mockDb
            .Received(1)
            .StringGet("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public void MultipleGet_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone()
    {
        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        _mockDb
            .StringGet(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns([RedisValue.Null, RedisValue.Null]);

        var result = _sut.Get<object>("key1", "key2");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.Where(o => o.IsSome).Should().HaveCount(0));
        _mockDb
            .Received(1)
            .StringGet(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [Test]
    public void Get_WhenDatabaseReturnsValidJson_ShouldReturnRightWithSome()
    {
        var redisReturn = new RedisValue("something");
        _mockDb
            .StringGet("key", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new TestData(1).ToOption());

        var result = _sut.Get<TestData>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.IsSome.Should().BeTrue();
                e.OnSome(d => d.Should().BeEquivalentTo(new TestData(1)));
            });
    }
}