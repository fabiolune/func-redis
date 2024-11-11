namespace Func.Redis.Tests.RedisKeyService;

public partial class RedisKeyServiceTests
{
    [Test]
    public async Task GetAsync_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockSourcesProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Key.RedisKeyService(_mockSourcesProvider, _mockSerDes);

        var result = await _sut.GetAsync<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public async Task MultipleGetAsync_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockSourcesProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Key.RedisKeyService(_mockSourcesProvider, _mockSerDes);

        var result = await _sut.GetAsync<object>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public async Task GetAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .StringGetAsync("key", Arg.Any<CommandFlags>())
            .Returns<RedisValue>(_ => throw exception);

        var result = await _sut.GetAsync<object>("key");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .StringGetAsync("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task MultipleGetAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        _mockDb
            .StringGetAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns<RedisValue[]>(_ => throw exception);

        var result = await _sut.GetAsync<object>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .StringGetAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [TestCase(null)]
    [TestCase("")]
    public async Task GetAsync_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string content)
    {
        _mockDb
            .StringGetAsync("key", Arg.Any<CommandFlags>())
            .Returns(new RedisValue(content));

        var result = await _sut.GetAsync<object>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        await _mockDb
            .Received(1)
            .StringGetAsync("key", Arg.Any<CommandFlags>());
    }

    [TestCase("", "")]
    [TestCase("", null)]
    [TestCase(null, null)]
    public async Task MultipleGetAsync_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string content1, string content2)
    {
        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        _mockDb
            .StringGetAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns([new RedisValue(content1), new RedisValue(content2)]);

        var result = await _sut.GetAsync<object>("key1", "key2");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.Should().HaveCount(2));
        await _mockDb
            .Received(1)
            .StringGetAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task GetAsync_WhenDatabaseReturnsNonJsonValue_ShouldReturnLeft()
    {
        var redisReturn = new RedisValue("something");
        var exception = new Exception("some message");
        _mockDb
            .StringGetAsync("key", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<object>(redisReturn)
            .Returns(_ => throw exception);

        var result = await _sut.GetAsync<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Message.Should().Be("some message"));
        await _mockDb
            .Received(1)
            .StringGetAsync("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task MultipleGetAsync_WhenSerDesFailsOnMultipleValues_ShouldReturnLeft()
    {
        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        var redisReturn1 = new RedisValue("first");
        var redisReturn2 = new RedisValue("second");
        _mockDb
            .StringGetAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns([redisReturn1, redisReturn2]);

        var exception1 = new Exception("first message");
        var exception2 = new Exception("second message");

        _mockSerDes
            .Deserialize<TestData>(redisReturn1)
            .Returns(_ => throw exception1);
        _mockSerDes
            .Deserialize<TestData>(redisReturn2)
            .Returns(_ => throw exception2);

        var result = await _sut.GetAsync<TestData>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Message.Should().Be("first message"));
        await _mockDb
            .Received(1)
            .StringGetAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task MultipleGetAsync_WhenSerDesFailsOnFirstValue_ShouldReturnLeft()
    {
        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        var redisReturn1 = new RedisValue("first");
        var redisReturn2 = new RedisValue("second");
        _mockDb
            .StringGetAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns([redisReturn1, redisReturn2]);

        var exception1 = new Exception("first message");

        _mockSerDes
            .Deserialize<TestData>(redisReturn1)
            .Returns(_ => throw exception1);
        _mockSerDes
            .Deserialize<TestData>(redisReturn2)
            .Returns(new TestData("some id").ToOption());

        var result = await _sut.GetAsync<TestData>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Message.Should().Be("first message"));
        await _mockDb
            .Received(1)
            .StringGetAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task MultipleGetAsync_WhenSerDesFailsOnAnotherValue_ShouldReturnLeft()
    {
        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        var redisReturn1 = new RedisValue("first");
        var redisReturn2 = new RedisValue("second");
        _mockDb
            .StringGetAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns([redisReturn1, redisReturn2]);

        var exception2 = new Exception("second message");

        _mockSerDes
            .Deserialize<TestData>(redisReturn1)
            .Returns(new TestData("some id").ToOption());
        _mockSerDes
            .Deserialize<TestData>(redisReturn2)
            .Returns(_ => throw exception2);

        var result = await _sut.GetAsync<TestData>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Message.Should().Be("second message"));
        await _mockDb
            .Received(1)
            .StringGetAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task GetAsync_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone()
    {
        _mockDb
            .StringGetAsync("key", Arg.Any<CommandFlags>())
            .Returns(RedisValue.Null);

        var result = await _sut.GetAsync<object>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        await _mockDb
            .Received(1)
            .StringGetAsync("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task MultipleGetAsync_WhenDatabaseReturnsNullJsonValue_ShouldReturnRightWithNone()
    {
        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        _mockDb
            .StringGetAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns([RedisValue.Null, RedisValue.Null]);

        var result = await _sut.GetAsync<object>("key1", "key2");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.Where(o => o.IsSome).Should().HaveCount(0));
        await _mockDb
            .Received(1)
            .StringGetAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task GetAsync_WhenDatabaseReturnsValidJson_ShouldReturnRightWithSome()
    {
        var redisReturn = new RedisValue("something");
        _mockDb
            .StringGetAsync("key", Arg.Any<CommandFlags>())
            .Returns(redisReturn);
        _mockSerDes
            .Deserialize<TestData>(redisReturn)
            .Returns(new TestData("some-id").ToOption());

        var result = await _sut.GetAsync<TestData>("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e =>
            {
                e.IsSome.Should().BeTrue();
                e.OnSome(d => d.Should().BeEquivalentTo(new TestData("some-id")));
            });
    }
}