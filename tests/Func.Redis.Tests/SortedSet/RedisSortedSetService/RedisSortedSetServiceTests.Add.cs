namespace Func.Redis.Tests.SortedSet.RedisSortedSetService;

internal partial class RedisSortedSetServiceTests
{
    [Test]
    public void Add_WhenDatabaseReturnsTrue_ShouldReturnSome()
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
    public async Task AddAsync_WhenDatabaseReturnsTrue_ShouldReturnSome()
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

    [Test]
    public void Add_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);

        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SortedSetAdd("key", "serialized", 1)
            .Returns(_ => throw exception);

        var result = _sut.Add("key", data, 1);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(error));
    }

    [Test]
    public async Task AddAsync_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);

        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SortedSetAddAsync("key", "serialized", 1)
            .Returns<bool>(_ => throw exception);

        var result = await _sut.AddAsync("key", data, 1);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(error));
    }

    [TestCase(0L)]
    [TestCase(1L)]
    [TestCase(2L)]
    public void MultipleAdd_WhenDatabaseReturnsLong_ShouldBeSome(long value)
    {
        var data1 = new TestData(1);
        var data2 = new TestData(2);

        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"data1");

        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"data2");

        _mockDb
            .SortedSetAdd("key", Arg.Is<SortedSetEntry[]>(entries =>
                entries.Length == 2 &&
                entries[0].Element == "data1" &&
                entries[1].Element == "data2"))
            .Returns(value);

        var result = _sut.Add("key", [(data1, 1.0), (data2, 2.0)]);

        _mockDb
            .Received(1)
            .SortedSetAdd("key", Arg.Is<SortedSetEntry[]>(entries =>
                entries.Length == 2 &&
                entries[0].Element == "data1" &&
                entries[1].Element == "data2"));

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void MultipleAdd_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);

        var data1 = new TestData(1);
        var data2 = new TestData(2);

        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"data1");

        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"data2");

        _mockDb
            .SortedSetAdd("key", Arg.Is<SortedSetEntry[]>(entries =>
                entries.Length == 2 &&
                entries[0].Element == "data1" &&
                entries[1].Element == "data2"))
            .Returns(_ => throw exception);

        var result = _sut.Add("key", [(data1, 1.0), (data2, 2.0)]);

        _mockDb
            .Received(1)
            .SortedSetAdd("key", Arg.Is<SortedSetEntry[]>(entries =>
                entries.Length == 2 &&
                entries[0].Element == "data1" &&
                entries[1].Element == "data2"));

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(error));
    }

    [TestCase(0L)]
    [TestCase(1L)]
    [TestCase(2L)]
    public async Task MultipleAddAsync_WhenDatabaseReturnsLong_ShouldBeSome(long value)
    {
        var data1 = new TestData(1);
        var data2 = new TestData(2);

        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"data1");

        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"data2");

        _mockDb
            .SortedSetAddAsync("key", Arg.Is<SortedSetEntry[]>(entries =>
                entries.Length == 2 &&
                entries[0].Element == "data1" &&
                entries[1].Element == "data2"))
            .Returns(value);

        var result = await _sut.AddAsync("key", [(data1, 1.0), (data2, 2.0)]);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task MultipleAddAsync_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);

        var data1 = new TestData(1);
        var data2 = new TestData(2);

        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"data1");

        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"data2");

        _mockDb
            .SortedSetAddAsync("key", Arg.Is<SortedSetEntry[]>(entries =>
                entries.Length == 2 &&
                entries[0].Element == "data1" &&
                entries[1].Element == "data2"))
            .Returns<long>(_ => throw exception);

        var result = await _sut.AddAsync("key", [(data1, 1.0), (data2, 2.0)]);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(error));
    }
}
