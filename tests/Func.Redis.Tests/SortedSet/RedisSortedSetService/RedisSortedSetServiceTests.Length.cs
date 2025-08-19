namespace Func.Redis.Tests.SortedSet.RedisSortedSetService;

internal partial class RedisSortedSetServiceTests
{
    [Test]
    public void Length_WhenDataBaseReturnsValue_ShouldReturnSome()
    {
        _mockDb
            .SortedSetLength("test_key")
            .Returns(5);

        var result = _sut.Length("test_key");

        result.IsRight.Should().BeTrue();
        result.OnRight(length => length.Should().Be(5));
    }

    [Test]
    public void Length_WhenDataBaseThrowsException_ShouldReturnError()
    {
        _mockDb
            .SortedSetLength("test_key")
            .Returns(_ => throw new Exception("Redis error"));

        var result = _sut.Length("test_key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }

    [Test]
    public void LengthByScore_WhenDataBaseReturnsValue_ShouldReturnSome()
    {
        _mockDb
            .SortedSetLength("test_key", 1.0, 10.0)
            .Returns(3);

        var result = _sut.LengthByScore("test_key", 1.0, 10.0);

        result.IsRight.Should().BeTrue();
        result.OnRight(length => length.Should().Be(3));
    }

    [Test]
    public void LengthByScore_WhenDataBaseThrowsException_ShouldReturnError()
    {
        _mockDb
            .SortedSetLength("test_key", 1.0, 10.0)
            .Returns(_ => throw new Exception("Redis error"));

        var result = _sut.LengthByScore("test_key", 1.0, 10.0);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }

    [Test]
    public void LengthByValue_WhenDataBaseReturnsValue_ShouldReturnSome()
    {
        var min = new TestData(1);
        var max = new TestData(10);

        _mockSerDes
            .Serialize(min)
            .Returns((RedisValue)"min_serialized");
        _mockSerDes
            .Serialize(max)
            .Returns((RedisValue)"max_serialized");
        _mockDb
            .SortedSetLengthByValue("test_key", "min_serialized", "max_serialized")
            .Returns(4);
        var result = _sut.LengthByValue("test_key", min, max);
        result.IsRight.Should().BeTrue();
        result.OnRight(length => length.Should().Be(4));
    }

    [Test]
    public void LengthByValue_WhenDataBaseThrowsException_ShouldReturnError()
    {
        var min = new TestData(1);
        var max = new TestData(10);

        _mockSerDes
            .Serialize(min)
            .Returns((RedisValue)"min_serialized");
        _mockSerDes
            .Serialize(max)
            .Returns((RedisValue)"max_serialized");
        _mockDb
            .SortedSetLengthByValue("test_key", "min_serialized", "max_serialized")
            .Returns(_ => throw new Exception("Redis error"));

        var result = _sut.LengthByValue("test_key", min, max);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }

    [Test]
    public async Task LengthAsync_WhenDataBaseReturnsValue_ShouldReturnSome()
    {
        _mockDb
            .SortedSetLengthAsync("test_key")
            .Returns(Task.FromResult(5L));

        var result = await _sut.LengthAsync("test_key");

        result.IsRight.Should().BeTrue();
        result.OnRight(length => length.Should().Be(5));
    }

    [Test]
    public async Task LengthAsync_WhenDataBaseThrowsException_ShouldReturnError()
    {
        _mockDb
            .SortedSetLengthAsync("test_key")
            .Returns<long>(_ => throw new Exception("Redis error"));

        var result = await _sut.LengthAsync("test_key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }

    [Test]
    public async Task LengthByScoreAsync_WhenDataBaseReturnsValue_ShouldReturnSome()
    {
        _mockDb
            .SortedSetLengthAsync("test_key", 1.0, 10.0)
            .Returns(3L);

        var result = await _sut.LengthByScoreAsync("test_key", 1.0, 10.0);

        result.IsRight.Should().BeTrue();
        result.OnRight(length => length.Should().Be(3));
    }

    [Test]
    public async Task LengthByScoreAsync_WhenDataBaseThrowsException_ShouldReturnError()
    {
        _mockDb
            .SortedSetLengthAsync("test_key", 1.0, 10.0)
            .Returns<long>(_ => throw new Exception("Redis error"));

        var result = await _sut.LengthByScoreAsync("test_key", 1.0, 10.0);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }

    [Test]
    public async Task LengthByValueAsync_WhenDataBaseReturnsValue_ShouldReturnSome()
    {
        var min = new TestData(1);
        var max = new TestData(10);

        _mockSerDes
            .Serialize(min)
            .Returns((RedisValue)"min_serialized");
        _mockSerDes
            .Serialize(max)
            .Returns((RedisValue)"max_serialized");
        _mockDb
            .SortedSetLengthByValueAsync("test_key", "min_serialized", "max_serialized")
            .Returns(4L);

        var result = await _sut.LengthByValueAsync("test_key", min, max);

        result.IsRight.Should().BeTrue();
        result.OnRight(length => length.Should().Be(4));
    }

    [Test]
    public async Task LengthByValueAsync_WhenDataBaseThrowsException_ShouldReturnError()
    {
        var min = new TestData(1);
        var max = new TestData(10);

        _mockSerDes
            .Serialize(min)
            .Returns((RedisValue)"min_serialized");
        _mockSerDes
            .Serialize(max)
            .Returns((RedisValue)"max_serialized");
        _mockDb
            .SortedSetLengthByValueAsync("test_key", "min_serialized", "max_serialized")
            .Returns<long>(_ => throw new Exception("Redis error"));

        var result = await _sut.LengthByValueAsync("test_key", min, max);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }
}
