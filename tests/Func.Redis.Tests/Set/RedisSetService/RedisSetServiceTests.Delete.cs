namespace Func.Redis.Tests.RedisSetService;
internal partial class RedisSetServiceTests
{
    [Test]
    public void Delete_WhenDatabaseReturnsTrue_ShouldReturnUnit()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SetRemove("key", "serialized", CommandFlags.None)
            .Returns(true);

        var result = _sut.Delete("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task DeleteAsync_WhenDatabaseReturnsTrue_ShouldReturnUnit()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SetRemoveAsync("key", "serialized", CommandFlags.None)
            .Returns(true);

        var result = await _sut.DeleteAsync("key", data);

        result.IsRight.Should().BeTrue();
    }

    [TestCase(0)]
    [TestCase(11)]
    public void MultipleDelete_WhenDatabaseReturnsTrue_ShouldReturnUnit(long returnValue)
    {
        var data1 = new TestData(1);
        var data2 = new TestData(2);

        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized 1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized 2");
        var values = new RedisValue[]
            {
            "serialized 1",
            "serialized 2"
            };
        _mockDb
            .SetRemove("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(values)), CommandFlags.None)
            .Returns(returnValue);

        var result = _sut.Delete("key", data1, data2);

        result.IsRight.Should().BeTrue();
    }

    [TestCase(0)]
    [TestCase(11)]
    public async Task MultipleDeleteAsync_WhenDatabaseReturnsTrue_ShouldReturnUnit(long returnValue)
    {
        var data1 = new TestData(1);
        var data2 = new TestData(2);

        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized 1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized 2");
        var values = new RedisValue[]
            {
            "serialized 1",
            "serialized 2"
            };
        _mockDb
            .SetRemoveAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(values)), CommandFlags.None)
            .Returns(returnValue);

        var result = await _sut.DeleteAsync("key", data1, data2);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void Delete_WhenDatabaseReturnsFalse_ShouldReturnRedisError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .SetRemove("key", "serialized", CommandFlags.None)
            .Returns(false);

        var result = _sut.Delete("key", data);

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis SREM Error")));
    }

    [Test]
    public async Task DeleteAsync_WhenDatabaseReturnsFalse_ShouldReturnRedisError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .SetRemoveAsync("key", "serialized", CommandFlags.None)
            .Returns(false);

        var result = await _sut.DeleteAsync("key", data);

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis SREM Error")));
    }

    [Test]
    public void Delete_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .SetRemove("key", "serialized", CommandFlags.None)
            .Returns(_ => throw new Exception("Redis Exception"));

        var result = _sut.Delete("key", data);

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public async Task DeleteAsync_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .SetRemoveAsync("key", "serialized", CommandFlags.None)
            .Returns<bool>(_ => throw new Exception("Redis Exception"));

        var result = await _sut.DeleteAsync("key", data);

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public void MultipleDelete_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        var data1 = new TestData(1);
        var data2 = new TestData(2);

        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized 1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized 2");
        var values = new RedisValue[]
            {
            "serialized 1",
            "serialized 2"
            };
        _mockDb
            .SetRemove("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(values)), CommandFlags.None)
            .Returns(_ => throw new Exception("Redis Exception"));

        var result = _sut.Delete("key", data1, data2);

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public async Task MultipleDeleteAsync_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        var data1 = new TestData(1);
        var data2 = new TestData(2);

        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized 1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized 2");
        var values = new RedisValue[]
            {
            "serialized 1",
            "serialized 2"
            };
        _mockDb
            .SetRemoveAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(values)), CommandFlags.None)
            .Returns<long>(_ => throw new Exception("Redis Exception"));

        var result = await _sut.DeleteAsync("key", data1, data2);

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }
}
