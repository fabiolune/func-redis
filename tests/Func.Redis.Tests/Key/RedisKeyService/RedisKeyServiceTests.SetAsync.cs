namespace Func.Redis.Tests.RedisKeyService;

public partial class RedisKeyServiceTests
{
    [Test]
    public async Task SetAsync_WhenDatabaseReturnsTrue_ShouldReturnUnit()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .StringSetAsync("key", "serialized", null, false, When.Always, CommandFlags.None)
            .Returns(true);

        var result = await _sut.SetAsync("key", data);

        result.IsRight.ShouldBeTrue();
    }

    [Test]
    public async Task MultipleSetAsync_WhenDatabaseReturnsTrue_ShouldReturnUnit()
    {
        var data1 = new TestData(1);
        var data2 = new TestData(2);
        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized 1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized 2");

        var pairs = new[]
            {
            new KeyValuePair<RedisKey, RedisValue>((RedisKey)"key1", (RedisValue)@"serialized 1"),
            new KeyValuePair<RedisKey, RedisValue>((RedisKey)"key2", (RedisValue)@"serialized 2")
        };

        _mockDb
            .StringSetAsync(Arg.Is<KeyValuePair<RedisKey, RedisValue>[]>(a => a.SequenceEqual(pairs)), When.Always, CommandFlags.None)
            .Returns(true);

        var result = await _sut.SetAsync(("key1", data1), ("key2", data2));

        result.IsRight.ShouldBeTrue();
    }

    [Test]
    public async Task SetAsync_WhenDatabaseReturnsFalse_ShouldReturnRedisError()
    {
        var data = new TestData(1);

        _mockDb
            .StringSetAsync("key", @"{""Id"":""some id""}", null, false, When.Always, CommandFlags.None)
            .Returns(false);

        var result = await _sut.SetAsync("key", data);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBeEquivalentTo(Error.New("Redis KEY SET Error")));
    }

    [Test]
    public async Task MultipleSetAsync_WhenDatabaseReturnsFalse_ShouldReturnRedisError()
    {
        var data1 = new TestData(1);
        var data2 = new TestData(2);

        var pairs = new[]
        {
            new KeyValuePair<RedisKey, RedisValue>((RedisKey)"key1", (RedisValue)@"{""Id"":""some id""}"),
            new KeyValuePair<RedisKey, RedisValue>((RedisKey)"key2", (RedisValue)@"{""Id"":""some other id""}")
        };

        _mockDb
            .StringSetAsync(Arg.Is<KeyValuePair<RedisKey, RedisValue>[]>(a => a.SequenceEqual(pairs)), When.Always, CommandFlags.None)
            .Returns(false);

        var result = await _sut.SetAsync(("key1", data1), ("key2", data2));

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBeEquivalentTo(Error.New("Redis KEY SET Error")));
    }

    [Test]
    public async Task SetAsync_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        var exception = new Exception("some message");
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .StringSetAsync("key", "serialized", null, false, When.Always, CommandFlags.None)
            .Returns<bool>(_ => throw exception);

        var result = await _sut.SetAsync("key", data);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
    }

    [Test]
    public async Task MultipleSetAsync_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        var exception = new Exception("some message");
        var data1 = new TestData(1);
        var data2 = new TestData(2);
        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized 1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized 2");

        _mockDb
            .StringSetAsync(Arg.Is<KeyValuePair<RedisKey, RedisValue>[]>(a => a.SequenceEqual(new[]
            {
            new KeyValuePair<RedisKey, RedisValue>((RedisKey)"key1", (RedisValue)@"serialized 1"),
            new KeyValuePair<RedisKey, RedisValue>((RedisKey)"key2", (RedisValue)@"serialized 2")
            })), When.Always, CommandFlags.None)
            .Returns(Task.FromException<bool>(exception));

        var result = await _sut.SetAsync(("key1", data1), ("key2", data2));

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
    }
}