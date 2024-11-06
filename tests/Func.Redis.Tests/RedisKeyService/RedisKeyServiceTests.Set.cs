namespace Func.Redis.Tests.RedisKeyService;

public partial class RedisKeyServiceTests
{
    [Test]
    public void Set_WhenDatabaseReturnsTrue_ShouldReturnUnit()
    {
        var data = new TestData("some id");
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .StringSet("key", "serialized", null, false, When.Always, CommandFlags.None)
            .Returns(true);

        var result = _sut.Set("key", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void MultipleSet_WhenDatabaseReturnsTrue_ShouldReturnUnit()
    {
        var data1 = new TestData("some id");
        var data2 = new TestData("some other id");

        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized 1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized 2");
        var values = new KeyValuePair<RedisKey, RedisValue>[]
            {
            new((RedisKey)"key1", (RedisValue)"serialized 1"),
            new((RedisKey)"key2", (RedisValue)"serialized 2")
            };
        _mockDb
            .StringSet(Arg.Is<KeyValuePair<RedisKey, RedisValue>[]>(v => v.SequenceEqual(values)), When.Always, CommandFlags.None)
            .Returns(true);

        var result = _sut.Set(("key1", data1), ("key2", data2));

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void Set_WhenDatabaseReturnsFalse_ShouldReturnRedisError()
    {
        var data = new TestData("some id");

        _mockDb
            .StringSet("key", @"{""Id"":""some id""}", null, false, When.Always, CommandFlags.None)
            .Returns(false);

        var result = _sut.Set("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Error setting value")));
    }

    [Test]
    public void MultipleSet_WhenDatabaseReturnsFalse_ShouldReturnRedisError()
    {
        var data1 = new TestData("some id");
        var data2 = new TestData("some other id");

        _mockDb
            .StringSet([
                new KeyValuePair<RedisKey, RedisValue>((RedisKey)"key1", (RedisValue)@"{""Id"":""some id""}"),
            new KeyValuePair<RedisKey, RedisValue>((RedisKey)"key2", (RedisValue)@"{""Id"":""some other id""}")
            ], When.Always, CommandFlags.None)
            .Returns(false);

        var result = _sut.Set(("key1", data1), ("key2", data2));

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Error setting value")));
    }

    [Test]
    public void Set_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        var exception = new Exception("some message");
        var data = new TestData("some id");
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");

        _mockDb
            .StringSet("key", "serialized", null, false, When.Always, CommandFlags.None)
            .Returns(_ => throw exception);

        var result = _sut.Set("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }

    [Test]
    public void MultipleSet_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        var exception = new Exception("some message");
        var data1 = new TestData("some id");
        var data2 = new TestData("some other id");
        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized 1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized 2");

        var values = new KeyValuePair<RedisKey, RedisValue>[]
            {
            new((RedisKey)"key1", (RedisValue)"serialized 1"),
            new((RedisKey)"key2", (RedisValue)"serialized 2")
            };
        _mockDb
            .StringSet(Arg.Is<KeyValuePair<RedisKey, RedisValue>[]>(v => v.SequenceEqual(values)), When.Always, CommandFlags.None)
            .Returns(_ => throw exception);

        var result = _sut.Set(("key1", data1), ("key2", data2));

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }
}