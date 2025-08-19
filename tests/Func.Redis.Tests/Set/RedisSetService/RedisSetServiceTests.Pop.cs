namespace Func.Redis.Tests.RedisSetService;
internal partial class RedisSetServiceTests
{
    [Test]
    public void Pop_WhenDatabaseReturnsValue_ShouldReturnSomeValue()
    {
        var data = new TestData(1);
        _mockSerDes
            .Deserialize<TestData>("serialized")
            .Returns(data.ToOption());
        _mockDb
            .SetPop("key", CommandFlags.None)
            .Returns((RedisValue)"serialized");

        var result = _sut.Pop<TestData>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(r =>
        {
            r.IsSome.Should().BeTrue();
            r.OnSome(v => v.Should().BeEquivalentTo(data));
        });
    }

    [Test]
    public void Pop_WhenDatabaseReturnsNull_ShouldReturnNone()
    {
        _mockDb
            .SetPop("key", CommandFlags.None)
            .Returns(RedisValue.Null);

        var result = _sut.Pop<TestData>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.IsNone.Should().BeTrue());
    }

    [Test]
    public async Task PopAsync_WhenDatabaseReturnsValue_ShouldReturnSomeValue()
    {
        var data = new TestData(1);
        _mockSerDes
            .Deserialize<TestData>("serialized")
            .Returns(data.ToOption());
        _mockDb
            .SetPopAsync("key", CommandFlags.None)
            .Returns((RedisValue)"serialized");

        var result = await _sut.PopAsync<TestData>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(r =>
        {
            r.IsSome.Should().BeTrue();
            r.OnSome(v => v.Should().BeEquivalentTo(data));
        });
    }

    [Test]
    public async Task PopAsync_WhenDatabaseReturnsNull_ShouldReturnNone()
    {
        _mockDb
            .SetPopAsync("key", CommandFlags.None)
            .Returns(RedisValue.Null);

        var result = await _sut.PopAsync<TestData>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.IsNone.Should().BeTrue());
    }

    [Test]
    public void Pop_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        _mockDb
            .SetPop("key", CommandFlags.None)
            .Returns(_ => throw new Exception("Redis Exception"));

        var result = _sut.Pop<TestData>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public async Task PopAsync_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        _mockDb
            .SetPopAsync("key", CommandFlags.None)
            .Returns<RedisValue>(_ => throw new Exception("Redis Exception"));

        var result = await _sut.PopAsync<TestData>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public void Pop_WhenDeserializeThrowsException_ShouldReturnRedisError()
    {
        _mockSerDes
            .Deserialize<TestData>("serialized")
            .Returns(_ => throw new Exception("Deserialize Exception"));
        _mockDb
            .SetPop("key", CommandFlags.None)
            .Returns((RedisValue)"serialized");

        var result = _sut.Pop<TestData>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Deserialize Exception")));
    }

    [Test]
    public async Task PopAsync_WhenDeserializeThrowsException_ShouldReturnRedisError()
    {
        _mockSerDes
            .Deserialize<TestData>("serialized")
            .Returns(_ => throw new Exception("Deserialize Exception"));
        _mockDb
            .SetPopAsync("key", CommandFlags.None)
            .Returns((RedisValue)"serialized");

        var result = await _sut.PopAsync<TestData>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Deserialize Exception")));
    }
}
