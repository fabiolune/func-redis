namespace Func.Redis.Tests.RedisSetService;
internal partial class RedisSetServiceTests
{
    [Test]
    public void Add_WhenDatabaseReturnsTrue_ShouldReturnUnit()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SetAdd("key", "serialized", CommandFlags.None)
            .Returns(true);

        var result = _sut.Add("key", data);

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
            .SetAddAsync("key", "serialized", CommandFlags.None)
            .Returns(Task.FromResult(true));

        var result = await _sut.AddAsync("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void Add_WhenDatabaseReturnsFalse_ShouldReturnRedisError()
    {
        var data = new TestData(1);

        _mockDb
            .SetAdd("key", @"{""Id"":""some id""}", CommandFlags.None)
            .Returns(false);

        var result = _sut.Add("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis SADD Error")));
    }

    [Test]
    public async Task AddAsync_WhenDatabaseReturnsFalse_ShouldReturnRedisError()
    {
        var data = new TestData(1);

        _mockDb
            .SetAddAsync("key", @"{""Id"":""some id""}", CommandFlags.None)
            .Returns(Task.FromResult(false));

        var result = await _sut.AddAsync("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis SADD Error")));
    }

    [Test]
    public void Add_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        var exception = new Exception("some message");
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SetAdd("key", "serialized", CommandFlags.None)
            .Returns(_ => throw exception);

        var result = _sut.Add("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("some message")));
    }

    [Test]
    public async Task AddAsync_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        var exception = new Exception("some message");
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .SetAddAsync("key", "serialized", CommandFlags.None)
            .Returns<bool>(_ => throw exception);

        var result = await _sut.AddAsync("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("some message")));
    }

    [Test]
    public void Add_WhenDataIsNull_ShouldReturnRedisError()
    {
        TestData data = null;

        var result = _sut.Add("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis SADD Error")));
    }

    [Test]
    public async Task AddAsync_WhenDataIsNull_ShouldReturnRedisError()
    {
        TestData data = null;

        var result = await _sut.AddAsync("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis SADD Error")));
    }
}
