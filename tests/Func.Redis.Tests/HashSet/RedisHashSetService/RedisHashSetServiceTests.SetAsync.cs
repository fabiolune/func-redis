using static Func.Redis.Tests.TestDataElements;

namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [TestCase(true)]
    [TestCase(false)]
    public async Task SetAsync_WhenDatabaseReturnsValidBoolean_ShouldReturnUnit(bool returnValue)
    {
        var data = new TestData(1);
        _mockDb
            .HashSetAsync("key", "field", @"{""Id"":""some id""}", When.Always, CommandFlags.None)
            .Returns(returnValue);

        var result = await _sut.SetAsync("key", "field", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task MultipleSetAsync_WhenDatabaseReturnsVerifiable_ShouldReturnUnit()
    {
        var data1 = new TestData(1);
        var data2 = new TestData(2);

        var called = false;
        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized 1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized 2");
        _mockDb
            .When(m => m.HashSetAsync("key", Arg.Is<HashEntry[]>(h =>
                h.SequenceEqual(new[]
                {
                    new HashEntry("field1", "serialized 1"),
                    new HashEntry("field2", "serialized 2")
                })), CommandFlags.None))
            .Do(m => called = true);

        var result = await _sut.SetAsync("key", ("field1", data1), ("field2", data2));

        result.IsRight.Should().BeTrue();
        called.Should().BeTrue();
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
            .HashSetAsync("key", "field", "serialized", When.Always, CommandFlags.None)
            .Returns<bool>(_ => throw exception);

        var result = await _sut.SetAsync("key", "field", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
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
            .When(m => m.HashSetAsync("key", Arg.Is<HashEntry[]>(h => h.SequenceEqual(new[] { new HashEntry("field1", "serialized 1"), new HashEntry("field2", "serialized 2") })), CommandFlags.None))
            .Do(_ => throw exception);

        var result = await _sut.SetAsync("key", ("field1", data1), ("field2", data2));

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }
}