namespace Func.Redis.Tests.SortedSet.RedisSortedSetService;
internal partial class RedisSortedSetServiceTests
{
    [Test]
    public void Decrement_WhenDataBaseReturnsValue_ShouldReturnUnit()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");

        _mockDb
            .SortedSetDecrement("test_key", "serialized_data", 1.0)
            .Returns(1.0);

        var result = _sut.Decrement("test_key", data, 1.0);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void Decrement_WhenDataBaseThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");
        _mockDb
            .SortedSetDecrement("test_key", "serialized_data", 1.0)
            .Returns(_ => throw new Exception("Redis error"));

        var result = _sut.Decrement("test_key", data, 1.0);
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }

    [Test]
    public void Decrement_WhenSerializerThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns(_ => throw new Exception("Serialization error"));

        var result = _sut.Decrement("test_key", data, 1.0);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Serialization error"));
    }

    [Test]
    public async Task DecrementAsync_WhenDataBaseReturnsValue_ShouldReturnUnit()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");
        _mockDb
            .SortedSetDecrementAsync("test_key", "serialized_data", 1.0)
            .Returns(1.0);

        var result = await _sut.DecrementAsync("test_key", data, 1.0);
        
        result.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task DecrementAsync_WhenDataBaseThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");
        _mockDb
            .SortedSetDecrementAsync("test_key", "serialized_data", 1.0)
            .Returns<double>(_ => throw new Exception("Redis error"));
     
        var result = await _sut.DecrementAsync("test_key", data, 1.0);
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }

    [Test]
    public async Task DecrementAsync_WhenSerializerThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns(_ => throw new Exception("Serialization error"));
        
        var result = await _sut.DecrementAsync("test_key", data, 1.0);
     
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Serialization error"));
    }
}
