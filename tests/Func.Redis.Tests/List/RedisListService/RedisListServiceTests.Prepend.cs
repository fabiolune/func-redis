namespace Func.Redis.Tests.RedisListService;

internal partial class RedisListServiceTests
{
    [Test]
    public void Prepend_WhenDatabaseReturnsLong_ShouldReturnRight()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .ListLeftPush("key", "serialized")
            .Returns(27L);

        var result = _sut.Prepend("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task PrependAsync_WhenDatabaseReturnsLong_ShouldReturnRight()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .ListLeftPushAsync("key", "serialized")
            .Returns(27L);

        var result = await _sut.PrependAsync("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void Prepend_WhenDatabaseThrowsException_ShouldReturnLeft()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .ListLeftPush("key", "serialized")
            .Returns(_ => throw new Exception("Redis Exception"));

        var result = _sut.Prepend("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public async Task PrependAsync_WhenDatabaseThrowsException_ShouldReturnLeft()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .ListLeftPushAsync("key", "serialized")
            .Returns<long>(_ => throw new Exception("Redis Exception"));

        var result = await _sut.PrependAsync("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public void MultiplePrepend_WhenDatabaseReturnsLong_ShouldReturnRight()
    {
        var value1 = new TestData(1);
        var value2 = new TestData(2);
        var data = new[] { value1, value2 };
        _mockSerDes
            .Serialize(value1)
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(value2)
            .Returns((RedisValue)"serialized2");
        var serialized = new RedisValue[] { "serialized1", "serialized2" };
        _mockDb
            .ListLeftPush("key", serialized)
            .Returns(27L);

        var result = _sut.Prepend("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task MultiplePrependAsync_WhenDatabaseReturnsLong_ShouldReturnRight()
    {
        var value1 = new TestData(1);
        var value2 = new TestData(2);
        var data = new[] { value1, value2 };
        _mockSerDes
            .Serialize(value1)
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(value2)
            .Returns((RedisValue)"serialized2");
        var serialized = new RedisValue[] { "serialized1", "serialized2" };
        _mockDb
            .ListLeftPushAsync("key", serialized)
            .Returns(27L);

        var result = await _sut.PrependAsync("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void MultiplePrepend_WhenDatabaseThrowsException_ShouldReturnLeft()
    {
        var value1 = new TestData(1);
        var value2 = new TestData(2);
        var data = new[] { value1, value2 };
        _mockSerDes
            .Serialize(value1)
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(value2)
            .Returns((RedisValue)"serialized2");
        var serialized = new RedisValue[] { "serialized1", "serialized2" };
        _mockDb
            .ListLeftPush("key", Arg.Is<RedisValue[]>(rv => rv.SequenceEqual(serialized)))
            .Returns(_ => throw new Exception("Redis Exception"));

        var result = _sut.Prepend("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public async Task MultiplePrependAsync_WhenDatabaseThrowsException_ShouldReturnLeft()
    {
        var value1 = new TestData(1);
        var value2 = new TestData(2);
        var data = new[] { value1, value2 };
        _mockSerDes
            .Serialize(value1)
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(value2)
            .Returns((RedisValue)"serialized2");
        var serialized = new RedisValue[] { "serialized1", "serialized2" };
        _mockDb
            .ListLeftPushAsync("key", Arg.Is<RedisValue[]>(rv => rv.SequenceEqual(serialized)))
            .Returns<long>(_ => throw new Exception("Redis Exception"));

        var result = await _sut.PrependAsync("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }
}