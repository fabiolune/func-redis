namespace Func.Redis.Tests.SortedSet.RedisSortedSetService;

internal partial class RedisSortedSetServiceTests
{
    [Test]
    public void Remove_WhenDatabaseReturnsTrue_ShouldReturnRight()
    {
        var data = new TestData(1);

        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SortedSetRemove("key", "serialized")
            .Returns(true);

        var result = _sut.Remove("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void Remove_WhenDatabaseReturnsFalse_ShouldReturnError()
    {
        var data = new TestData(1);

        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SortedSetRemove("key", "serialized")
            .Returns(false);

        var result = _sut.Remove("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("Redis ZREM Error"));
    }

    [Test]
    public void Remove_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);

        var data = new TestData(1);

        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SortedSetRemove("key", "serialized")
            .Returns(_ => throw exception);

        var result = _sut.Remove("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
    }

    [Test]
    public async Task RemoveAsync_WhenDatabaseReturnsTrue_ShouldReturnRight()
    {
        var data = new TestData(1);

        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SortedSetRemoveAsync("key", "serialized")
            .Returns(true);

        var result = await _sut.RemoveAsync("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task RemoveAsync_WhenDatabaseReturnsFalse_ShouldReturnError()
    {
        var data = new TestData(1);

        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SortedSetRemoveAsync("key", "serialized")
            .Returns(false);

        var result = await _sut.RemoveAsync("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("Redis ZREM Error"));
    }

    [Test]
    public async Task RemoveAsync_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);

        var data = new TestData(1);

        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SortedSetRemoveAsync("key", "serialized")
            .Returns<bool>(_ => throw exception);

        var result = await _sut.RemoveAsync("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
    }

    [TestCase(0L)]
    [TestCase(1L)]
    [TestCase(2L)]
    public void RemoveMultiple_WhenDatabaseReturnsLong_ShouldReturnRight(long removeResult)
    {
        var data = new[] { new TestData(1), new TestData(2) };
        _mockSerDes
            .Serialize(data[0])
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(data[1])
            .Returns((RedisValue)"serialized2");
        var serialized = new RedisValue[] { "serialized1", "serialized2" };
        _mockDb
            .SortedSetRemove("key", Arg.Is<RedisValue[]>(a => a.SequenceEqual(serialized)))
            .Returns(removeResult);

        var result = _sut.Remove<TestData>("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void RemoveMultiple_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        var data = new[] { new TestData(1), new TestData(2) };
        _mockSerDes
            .Serialize(data[0])
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(data[1])
            .Returns((RedisValue)"serialized2");
        var serialized = new RedisValue[] { "serialized1", "serialized2" };
        _mockDb
            .SortedSetRemove("key", Arg.Is<RedisValue[]>(a => a.SequenceEqual(serialized)))
            .Returns(_ => throw exception);

        var result = _sut.Remove<TestData>("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
    }

    [TestCase(0L)]
    [TestCase(1L)]
    [TestCase(2L)]
    public async Task RemoveMultipleAsync_WhenDatabaseReturnsLong_ShouldReturnRight(long removeResult)
    {
        var data = new[] { new TestData(1), new TestData(2) };
        _mockSerDes
            .Serialize(data[0])
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(data[1])
            .Returns((RedisValue)"serialized2");
        var serialized = new RedisValue[] { "serialized1", "serialized2" };
        _mockDb
            .SortedSetRemoveAsync("key", Arg.Is<RedisValue[]>(a => a.SequenceEqual(serialized)))
            .Returns(removeResult);

        var result = await _sut.RemoveAsync<TestData>("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task RemoveMultipleAsync_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        var data = new[] { new TestData(1), new TestData(2) };
        _mockSerDes
            .Serialize(data[0])
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(data[1])
            .Returns((RedisValue)"serialized2");
        var serialized = new RedisValue[] { "serialized1", "serialized2" };
        _mockDb
            .SortedSetRemoveAsync("key", Arg.Is<RedisValue[]>(a => a.SequenceEqual(serialized)))
            .Returns<long>(_ => throw exception);

        var result = await _sut.RemoveAsync<TestData>("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
    }

    [TestCase(0L)]
    [TestCase(1L)]
    [TestCase(1_000_000L)]
    public void RemoveRangeByScore_WhenDatabaseReturnsTrue_ShouldReturnRight(long value)
    {
        _mockDb
            .SortedSetRemoveRangeByScore("key", 1, 10)
            .Returns(value);

        var result = _sut.RemoveRangeByScore("key", 1, 10);

        result.IsRight.Should().BeTrue();
    }

    [TestCase(0L)]
    [TestCase(1L)]
    [TestCase(1_000_000L)]
    public async Task RemoveRangeByScoreAsync_WhenDatabaseReturnsTrue_ShouldReturnRight(long value)
    {
        _mockDb
            .SortedSetRemoveRangeByScoreAsync("key", 1, 10)
            .Returns(value);
        var result = await _sut.RemoveRangeByScoreAsync("key", 1, 10);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void RemoveRangeByScore_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);

        _mockDb
            .SortedSetRemoveRangeByScore("key", 1, 10)
            .Returns(_ => throw exception);
        var result = _sut.RemoveRangeByScore("key", 1, 10);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
    }

    [Test]
    public async Task RemoveRangeByScoreAsync_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);

        _mockDb
            .SortedSetRemoveRangeByScoreAsync("key", 1, 10)
            .Returns<long>(_ => throw exception);
        var result = await _sut.RemoveRangeByScoreAsync("key", 1, 10);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
    }

    [TestCase(0L)]
    [TestCase(1L)]
    [TestCase(1_000_000L)]
    public void RemoveRangeByValue_WhenDatabaseReturnsValue_ShouldReturnRight(long value)
    {
        var data1 = new TestData(1);
        var data2 = new TestData(2);
        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized2");
        _mockDb
            .SortedSetRemoveRangeByValue("key", "serialized1", "serialized2")
            .Returns(value);

        var result = _sut.RemoveRangeByValue("key", data1, data2);

        result.IsRight.Should().BeTrue();
    }

    [TestCase(0L)]
    [TestCase(1L)]
    [TestCase(1_000_000L)]
    public async Task RemoveRangeByValueAsync_WhenDatabaseReturnsValue_ShouldReturnRight(long value)
    {
        var data1 = new TestData(1);
        var data2 = new TestData(2);
        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized2");
        _mockDb
            .SortedSetRemoveRangeByValueAsync("key", "serialized1", "serialized2")
            .Returns(value);
        var result = await _sut.RemoveRangeByValueAsync("key", data1, data2);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void RemoveRangeByValue_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        var data1 = new TestData(1);
        var data2 = new TestData(2);
        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized2");
        _mockDb
            .SortedSetRemoveRangeByValue("key", "serialized1", "serialized2")
            .Returns(_ => throw exception);

        var result = _sut.RemoveRangeByValue("key", data1, data2);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
    }

    [Test]
    public async Task RemoveRangeByValueAsync_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        var data1 = new TestData(1);
        var data2 = new TestData(2);
        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized2");
        _mockDb
            .SortedSetRemoveRangeByValueAsync("key", "serialized1", "serialized2")
            .Returns<long>(_ => throw exception);

        var result = await _sut.RemoveRangeByValueAsync("key", data1, data2);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
    }

    [Test]
    public void RemoveRangeByValue_WhenSerializerThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        var data1 = new TestData(1);
        var data2 = new TestData(2);

        _mockSerDes
            .Serialize(data1)
            .Returns(_ => throw exception);

        var result = _sut.RemoveRangeByValue("key", data1, data2);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
    }

    [Test]
    public async Task RemoveRangeByValueAsync_WhenSerializerThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        var data1 = new TestData(1);
        var data2 = new TestData(2);

        _mockSerDes
            .Serialize(data1)
            .Returns(_ => throw exception);

        var result = await _sut.RemoveRangeByValueAsync("key", data1, data2);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
    }
}
