using static Func.Redis.Tests.TestDataElements;

namespace Func.Redis.Tests.SortedSet.RedisSortedSetService;

internal partial class RedisSortedSetServiceTests
{
    [Test]
    public void AddAsync_ShouldCallDbSortedSetAdd_WhenCalled()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SortedSetAdd("key", "serialized", 1)
            .Returns(true);

        var result = _sut.Add("key", data, 1);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task AddAsync_WhenDatabaseReturnsTrue_ShouldReturnUnit()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SortedSetAddAsync("key", "serialized", 1)
            .Returns(Task.FromResult(true));

        var result = await _sut.AddAsync("key", data, 1);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void Add_WhenDatabaseReturnsFalse_ShouldReturnRedisError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SortedSetAdd("key", "serialized", 1)
            .Returns(false);

        var result = _sut.Add("key", data, 1);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis ZADD Error")));
    }

    [Test]
    public async Task AddAsync_WhenDatabaseReturnsFalse_ShouldReturnRedisError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SortedSetAddAsync("key", "serialized", 1)
            .Returns(Task.FromResult(false));

        var result = await _sut.AddAsync("key", data, 1);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis ZADD Error")));
    }
}

