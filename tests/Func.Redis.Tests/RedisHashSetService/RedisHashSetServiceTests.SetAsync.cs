namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [TestCase("", "key", "field", true)]
    [TestCase(":", "key", "field", true)]
    [TestCase(" :", "key", "field", true)]
    [TestCase("::", "key", "field", true)]
    [TestCase("prefix", "prefix:key", "field", true)]
    [TestCase("prefix:", "prefix:key", "field", true)]
    [TestCase("prefix::", "prefix:key", "field", true)]
    [TestCase(":", "key", "field", false)]
    [TestCase(" :", "key", "field", false)]
    [TestCase("::", "key", "field", false)]
    [TestCase("prefix", "prefix:key", "field", false)]
    [TestCase("prefix:", "prefix:key", "field", false)]
    [TestCase("prefix::", "prefix:key", "field", false)]
    public async Task SetAsync_WhenDatabaseReturnsValidBoolean_ShouldReturnUnit(string prefix, string key, string field, bool returnValue)
    {
        var data = new TestData("some id");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashSetAsync(key, field, @"{""Id"":""some id""}", When.Always, CommandFlags.None)
            .Returns(returnValue);

        var result = await _sut.SetAsync("key", "field", data);

        result.IsRight.Should().BeTrue();
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("::", "key")]
    [TestCase(": ", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    [TestCase("prefix: ", "prefix:key")]
    public async Task MultipleSetAsync_WhenDatabaseReturnsVerifiable_ShouldReturnUnit(string prefix, string key)
    {
        const string field1 = "field1";
        const string field2 = "field2";
        var data1 = new TestData("id1");
        var data2 = new TestData("id2");
        var called = false;

        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized2");

        _mockDb
            .When(m => m.HashSetAsync(
                key, Arg.Is<HashEntry[]>(h =>
                    h.SequenceEqual(new[] { new HashEntry(field1, "serialized1"), new HashEntry(field2, "serialized2") })), CommandFlags.None))
            .Do(m => called = true);

        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        var result = await _sut.SetAsync("key", (field1, data1), (field2, data2));

        result.IsRight.Should().BeTrue();
        called.Should().BeTrue();
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase(" :", "key")]
    [TestCase("::", "key")]
    [TestCase("   ::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public async Task SetAsync_WhenDatabaseThrowsException_ShouldReturnRedisError(string prefix, string key)
    {
        var exception = new Exception("some message");
        var data = new TestData("some id");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .HashSetAsync(key, "field", "serialized", When.Always, CommandFlags.None)
            .Returns<Task<bool>>(_ => throw exception);

        var result = await _sut.SetAsync("key", "field", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("::", "key")]
    [TestCase(": ", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    [TestCase("prefix: ", "prefix:key")]
    public async Task MultipleSetAsync_WhenDatabaseThrowsException_ShouldReturnRedisError(string prefix, string key)
    {
        const string field1 = "field1";
        const string field2 = "field2";

        var exception = new Exception("some message");
        var data1 = new TestData("id1");
        var data2 = new TestData("id2");
        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized2");

        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashSetAsync(key, Arg.Is<HashEntry[]>(h =>
                h.SequenceEqual(new[] { new HashEntry(field1, "serialized1"), new HashEntry(field2, "serialized2") })), CommandFlags.None)
            .Returns(async _ => await Task.FromException(exception));

        var result = await _sut.SetAsync("key", (field1, data1), (field2, data2));

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }
}