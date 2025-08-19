namespace Func.Redis.Tests.SortedSet.RedisSortedSetService;
internal partial class RedisSortedSetServiceTests
{
    [Test]
    public void Increment_WhenDataBaseReturnsValue_ShouldReturnUnit()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");

        _mockDb
            .SortedSetIncrement("test_key", "serialized_data", 1.0)
            .Returns(1.0);

        var result = _sut.Increment("test_key", data, 1.0);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void Increment_WhenDataBaseThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");
        _mockDb
            .SortedSetIncrement("test_key", "serialized_data", 1.0)
            .Returns(_ => throw new Exception("Redis error"));

        var result = _sut.Increment("test_key", data, 1.0);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }

    [Test]
    public void Increment_WhenSerializerThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns(_ => throw new Exception("Serialization error"));

        var result = _sut.Increment("test_key", data, 1.0);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Serialization error"));
    }

    [Test]
    public async Task IncrementAsync_WhenDataBaseReturnsValue_ShouldReturnUnit()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");
        _mockDb
            .SortedSetIncrementAsync("test_key", "serialized_data", 1.0)
            .Returns(1.0);

        var result = await _sut.IncrementAsync("test_key", data, 1.0);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task IncrementAsync_WhenDataBaseThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");
        _mockDb
            .SortedSetIncrementAsync("test_key", "serialized_data", 1.0)
            .Returns<double>(_ => throw new Exception("Redis error"));

        var result = await _sut.IncrementAsync("test_key", data, 1.0);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }

    [Test]
    public async Task IncrementAsync_WhenSerializerThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns(_ => throw new Exception("Serialization error"));

        var result = await _sut.IncrementAsync("test_key", data, 1.0);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Serialization error"));
    }
}
