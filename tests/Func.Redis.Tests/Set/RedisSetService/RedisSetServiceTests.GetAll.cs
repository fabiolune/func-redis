namespace Func.Redis.Tests.RedisSetService;
internal partial class RedisSetServiceTests
{
    [Test]
    public void GetAll_WhenDatabaseReturnsValues_ShouldReturnSomeValues()
    {
        var data1 = new TestData(1);
        var data2 = new TestData(2);
        var values = new[] { (RedisValue)"serialized 1", (RedisValue)"serialized 2" };

        _mockSerDes
            .Deserialize<TestData>("serialized 1")
            .Returns(data1.ToOption());
        _mockSerDes
            .Deserialize<TestData>("serialized 2")
            .Returns(data2.ToOption());
        _mockDb
            .SetMembers("key", CommandFlags.None)
            .Returns(values);

        var result = _sut.GetAll<TestData>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.Filter().Should().BeEquivalentTo([data1, data2]));
    }

    [Test]
    public void GetAll_WhenDatabaseReturnsNullOrEmptyString_ShouldReturnNone()
    {
        _mockDb
            .SetMembers("key", CommandFlags.None)
            .Returns([RedisValue.Null, RedisValue.Null, RedisValue.EmptyString]);

        var result = _sut.GetAll<TestData>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.Filter().Should().BeEmpty());
    }

    [Test]
    public async Task GetAllAsync_WhenDatabaseReturnsValues_ShouldReturnSomeValues()
    {
        var data1 = new TestData(1);
        var data2 = new TestData(2);
        var values = new[] { (RedisValue)"serialized 1", (RedisValue)"serialized 2" };

        _mockSerDes
            .Deserialize<TestData>("serialized 1")
            .Returns(data1.ToOption());
        _mockSerDes
            .Deserialize<TestData>("serialized 2")
            .Returns(data2.ToOption());
        _mockDb
            .SetMembersAsync("key", CommandFlags.None)
            .Returns(values);

        var result = await _sut.GetAllAsync<TestData>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.Filter().Should().BeEquivalentTo([data1, data2]));
    }

    [Test]
    public async Task GetAllAsync_WhenDatabaseReturnsNullOrEmptyString_ShouldReturnNone()
    {
        _mockDb
            .SetMembersAsync("key", CommandFlags.None)
            .Returns([RedisValue.Null, RedisValue.Null, RedisValue.EmptyString]);

        var result = await _sut.GetAllAsync<TestData>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.Filter().Should().BeEmpty());
    }

    [Test]
    public void GetAll_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        _mockDb
            .SetMembers("key", CommandFlags.None)
            .Returns(_ => throw new Exception("Redis ERROR"));

        var result = _sut.GetAll<TestData>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis ERROR")));
    }

    [Test]
    public async Task GetAllAsync_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        _mockDb
            .SetMembersAsync("key", CommandFlags.None)
            .Returns<RedisValue[]>(_ => throw new Exception("Redis ERROR"));

        var result = await _sut.GetAllAsync<TestData>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis ERROR")));
    }
}
