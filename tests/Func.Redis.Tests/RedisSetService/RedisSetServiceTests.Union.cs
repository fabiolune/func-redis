namespace Func.Redis.Tests.RedisSetService;
internal partial class RedisSetServiceTests
{
    [Test]
    public void Union_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockSourcesProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.RedisSetService(_mockSourcesProvider, _mockSerDes);

        _mockSourcesProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.Union<string>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public void Union_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .SetCombine(SetOperation.Union, "key1", "key2", CommandFlags.None)
            .Returns(_ => throw exception);

        var result = _sut.Union<string>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }

    [Test]
    public void Union_WhenDatabaseReturnsValues_ShouldReturnValues()
    {
        var values = new[] { (RedisValue)"value1", (RedisValue)"value2" };
        var deserialized = new[] { new TestData("1"), new TestData("2") };

        _mockSerDes
            .Deserialize<TestData>(values[0])
            .Returns(deserialized[0].ToOption());
        _mockSerDes
            .Deserialize<TestData>(values[1])
            .Returns(deserialized[1].ToOption());
        _mockDb
            .SetCombine(SetOperation.Union, "key1", "key2", CommandFlags.None)
            .Returns(values);

        var result = _sut.Union<TestData>("key1", "key2");

        result.IsRight.Should().BeTrue();
        result.OnRight(res => res.Should().BeEquivalentTo(deserialized));
    }

    [Test]
    public async Task UnionAsync_WhenDatabaseReturnsValues_ShouldReturnValues()
    {
        var values = new[] { (RedisValue)"value1", (RedisValue)"value2" };
        var deserialized = new[] { new TestData("1"), new TestData("2") };

        _mockSerDes
            .Deserialize<TestData>(values[0])
            .Returns(deserialized[0].ToOption());
        _mockSerDes
            .Deserialize<TestData>(values[1])
            .Returns(deserialized[1].ToOption());
        _mockDb
            .SetCombineAsync(SetOperation.Union, "key1", "key2", CommandFlags.None)
            .Returns(values);

        var result = await _sut.UnionAsync<TestData>("key1", "key2");

        result.IsRight.Should().BeTrue();
        result.OnRight(res => res.Should().BeEquivalentTo(deserialized));
    }

    [Test]
    public async Task UnionAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .SetCombineAsync(SetOperation.Union, "key1", "key2", CommandFlags.None)
            .Returns<RedisValue[]>(_ => throw exception);

        var result = await _sut.UnionAsync<string>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }
}