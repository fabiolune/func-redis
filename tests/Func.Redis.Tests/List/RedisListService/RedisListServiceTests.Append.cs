namespace Func.Redis.Tests.RedisListService;

internal partial class RedisListServiceTests
{
    [Test]
    public void Append_WhenDatabaseReturnsLong_ShouldReturnRight()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .ListRightPush("key", "serialized")
            .Returns(27L);

        var result = _sut.Append("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task AppendAsync_WhenDatabaseReturnsLong_ShouldReturnRight()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .ListRightPushAsync("key", "serialized")
            .Returns(27L);

        var result = await _sut.AppendAsync("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void Append_WhenDatabaseThrowsException_ShouldReturnRight()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .ListRightPush("key", "serialized")
            .Returns(_ => throw new Exception("Redis Exception"));

        var result = _sut.Append("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public async Task AppendAsync_WhenDatabaseThrowsException_ShouldReturnRight()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .ListRightPushAsync("key", "serialized")
            .Returns<long>(_ => throw new Exception("Redis Exception"));

        var result = await _sut.AppendAsync("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public void MultipleAppend_WhenDatabaseReturnsLong_ShouldReturnRight()
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
            .ListRightPush("key", serialized)
            .Returns(27L);

        var result = _sut.Append("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task MultipleAppendAsync_WhenDatabaseReturnsLong_ShouldReturnRight()
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
            .ListRightPushAsync("key", serialized)
            .Returns(27L);

        var result = await _sut.AppendAsync("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void MultipleAppend_WhenDatabaseThrowsException_ShouldReturnRight()
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
            .ListRightPush("key", Arg.Is<RedisValue[]>(rv => rv.SequenceEqual(serialized)))
            .Returns(_ => throw new Exception("Redis Exception"));

        var result = _sut.Append("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public async Task MultipleAppendAsync_WhenDatabaseThrowsException_ShouldReturnRight()
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
            .ListRightPushAsync("key", Arg.Is<RedisValue[]>(rv => rv.SequenceEqual(serialized)))
            .Returns<long>(_ => throw new Exception("Redis Exception"));

        var result = await _sut.AppendAsync("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }
}