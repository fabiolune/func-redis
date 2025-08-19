namespace Func.Redis.Tests.SortedSet.RedisSortedSetService;
internal partial class RedisSortedSetServiceTests
{
    [Test]
    public void Intersect_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var keys = new[] { "key1", "key2" };
        var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
        _mockDb
            .SortedSetCombine(SetOperation.Intersect, Arg.Is<RedisKey[]>(a => a.SequenceEqual(redisKeys)))
            .Returns(_ => throw exception);

        var result = _sut.Intersect<object>(keys);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }

    [Test]
    public void Intersect_WhenDatabaseReturnsValues_ShouldReturnValues()
    {
        var keys = new[] { "key1", "key2" };
        var values = new[] { (RedisValue)"value1", (RedisValue)"value2" };
        var deserialized = new[] { new TestData(1), new TestData(2) };
        _mockSerDes
            .Deserialize<TestData>(values[0])
            .Returns(deserialized[0].ToOption());
        _mockSerDes
            .Deserialize<TestData>(values[1])
            .Returns(deserialized[1].ToOption());
        _mockDb
            .SortedSetCombine(SetOperation.Intersect, Arg.Is<RedisKey[]>(a => a.SequenceEqual(keys.Select(k => (RedisKey)k))))
            .Returns(values);
        var result = _sut.Intersect<TestData>(keys);
        result.IsRight.Should().BeTrue();
        result.OnRight(res => res.Should().BeEquivalentTo(deserialized));
    }

    [Test]
    public void Intersect_WhenDatabaseReturnsSomeNones_ShouldReturnFilteredValues()
    {
        var keys = new[] { "key1", "key2" };
        var values = new[] { (RedisValue)"value1", (RedisValue)"value2" };
        var deserialized = new[] { new TestData(1) };
        _mockSerDes
            .Deserialize<TestData>(values[0])
            .Returns(deserialized[0].ToOption());
        _mockSerDes
            .Deserialize<TestData>(values[1])
            .Returns(Option<TestData>.None());
        _mockDb
            .SortedSetCombine(SetOperation.Intersect, Arg.Is<RedisKey[]>(a => a.SequenceEqual(keys.Select(k => (RedisKey)k))))
            .Returns(values);
        var result = _sut.Intersect<TestData>(keys);
        result.IsRight.Should().BeTrue();
        result.OnRight(res => res.Should().BeEquivalentTo(deserialized));
    }

    [Test]
    public void Intersect_WhenSerializerThrowsException_ShouldReturnError()
    {
        var keys = new[] { "key1", "key2" };
        var values = new[] { (RedisValue)"value1", (RedisValue)"value2" };
        _mockDb
            .SortedSetCombine(SetOperation.Intersect, Arg.Is<RedisKey[]>(a => a.SequenceEqual(keys.Select(k => (RedisKey)k))))
            .Returns(values);
        _mockSerDes
            .Deserialize<TestData>(Arg.Any<RedisValue>())
            .Returns(_ => throw new Exception("Serialization error"));

        var result = _sut.Intersect<TestData>(keys);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Serialization error"));
    }

    [Test]
    public async Task IntersectAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var keys = new[] { "key1", "key2" };
        var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
        _mockDb
            .SortedSetCombineAsync(SetOperation.Intersect, Arg.Is<RedisKey[]>(a => a.SequenceEqual(redisKeys)))
            .Returns<RedisValue[]>(_ => throw exception);

        var result = await _sut.IntersectAsync<object>(keys);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }

    [Test]
    public async Task IntersectAsync_WhenDatabaseReturnsValues_ShouldReturnValues()
    {
        var keys = new[] { "key1", "key2" };
        var values = new[] { (RedisValue)"value1", (RedisValue)"value2" };
        var deserialized = new[] { new TestData(1), new TestData(2) };
        _mockSerDes
            .Deserialize<TestData>(values[0])
            .Returns(deserialized[0].ToOption());
        _mockSerDes
            .Deserialize<TestData>(values[1])
            .Returns(deserialized[1].ToOption());
        _mockDb
            .SortedSetCombineAsync(SetOperation.Intersect, Arg.Is<RedisKey[]>(a => a.SequenceEqual(keys.Select(k => (RedisKey)k))))
            .Returns(Task.FromResult(values));

        var result = await _sut.IntersectAsync<TestData>(keys);

        result.IsRight.Should().BeTrue();
        result.OnRight(res => res.Should().BeEquivalentTo(deserialized));
    }

    [Test]
    public async Task IntersectAsync_WhenDatabaseReturnsSomeNones_ShouldReturnFilteredValues()
    {
        var keys = new[] { "key1", "key2" };
        var values = new[] { (RedisValue)"value1", (RedisValue)"value2" };
        var deserialized = new[] { new TestData(1) };
        _mockSerDes
            .Deserialize<TestData>(values[0])
            .Returns(deserialized[0].ToOption());
        _mockSerDes
            .Deserialize<TestData>(values[1])
            .Returns(Option<TestData>.None());
        _mockDb
            .SortedSetCombineAsync(SetOperation.Intersect, Arg.Is<RedisKey[]>(a => a.SequenceEqual(keys.Select(k => (RedisKey)k))))
            .Returns(Task.FromResult(values));

        var result = await _sut.IntersectAsync<TestData>(keys);

        result.IsRight.Should().BeTrue();
        result.OnRight(res => res.Should().BeEquivalentTo(deserialized));
    }

    [Test]
    public async Task IntersectAsync_WhenSerializerThrowsException_ShouldReturnError()
    {
        var keys = new[] { "key1", "key2" };
        var values = new[] { (RedisValue)"value1", (RedisValue)"value2" };
        _mockDb
            .SortedSetCombineAsync(SetOperation.Intersect, Arg.Is<RedisKey[]>(a => a.SequenceEqual(keys.Select(k => (RedisKey)k))))
            .Returns(Task.FromResult(values));
        _mockSerDes
            .Deserialize<TestData>(Arg.Any<RedisValue>())
            .Returns(_ => throw new Exception("Serialization error"));

        var result = await _sut.IntersectAsync<TestData>(keys);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Serialization error"));
    }
}
