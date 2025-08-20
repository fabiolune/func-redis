namespace Func.Redis.Tests.RedisSetService;
internal partial class RedisSetServiceTests
{
    [Test]
    public void Difference_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockSourcesProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.Set.RedisSetService(_mockSourcesProvider, _mockSerDes);

        _mockSourcesProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.Difference<string>("key1", "key2");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(err => err.ShouldBe(Error.New(new NullReferenceException())));
    }

    [Test]
    public void Difference_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .SetCombine(SetOperation.Difference, "key1", "key2", CommandFlags.None)
            .Returns(_ => throw exception);

        var result = _sut.Difference<string>("key1", "key2");

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
    }

    [Test]
    public void Difference_WhenDatabaseReturnsValues_ShouldReturnValues()
    {
        var values = new[] { (RedisValue)"value1", (RedisValue)"value2" };
        var deserialized = new[] { new TestData(1), new TestData(2) };

        _mockSerDes
            .Deserialize<TestData>(values[0])
            .Returns(deserialized[0].ToOption());
        _mockSerDes
            .Deserialize<TestData>(values[1])
            .Returns(deserialized[1].ToOption());
        _mockDb
            .SetCombine(SetOperation.Difference, "key1", "key2", CommandFlags.None)
            .Returns(values);

        var result = _sut.Difference<TestData>("key1", "key2");

        result.IsRight.ShouldBeTrue();
        result.OnRight(res => res.ShouldBeEquivalentTo(deserialized));
    }

    [Test]
    public async Task DifferenceAsync_WhenDatabaseReturnsValues_ShouldReturnValues()
    {
        var values = new[] { (RedisValue)"value1", (RedisValue)"value2" };
        var deserialized = new[] { new TestData(1), new TestData(2) };

        _mockSerDes
            .Deserialize<TestData>(values[0])
            .Returns(deserialized[0].ToOption());
        _mockSerDes
            .Deserialize<TestData>(values[1])
            .Returns(deserialized[1].ToOption());
        _mockDb
            .SetCombineAsync(SetOperation.Difference, "key1", "key2", CommandFlags.None)
            .Returns(values);

        var result = await _sut.DifferenceAsync<TestData>("key1", "key2");

        result.IsRight.ShouldBeTrue();
        result.OnRight(res => res.ShouldBeEquivalentTo(deserialized));
    }

    [Test]
    public async Task DifferenceAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .SetCombineAsync(SetOperation.Difference, "key1", "key2", CommandFlags.None)
            .Returns<RedisValue[]>(_ => throw exception);

        var result = await _sut.DifferenceAsync<string>("key1", "key2");

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
    }
}