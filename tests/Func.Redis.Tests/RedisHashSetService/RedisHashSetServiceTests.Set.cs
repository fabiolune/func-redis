namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [TestCase(true)]
    [TestCase(false)]
    public void Set_WhenDatabaseReturnsValidBoolean_ShouldReturnUnit(bool returnValue)
    {
        var data = new TestData("some id");
        _mockDb
            .HashSet("key", "field", @"{""Id"":""some id""}", When.Always, CommandFlags.None)
            .Returns(returnValue);

        var result = _sut.Set("key", "field", data);

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void MultipleSet_WhenDatabaseReturnsVerifiable_ShouldReturnUnit()
    {
        var data1 = new TestData("id1");
        var data2 = new TestData("id2");

        var called = false;
        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized 1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized 2");
        _mockDb
            .When(m => m.HashSet("key", Arg.Is<HashEntry[]>(h =>
                h.SequenceEqual(new[]
                {
                    new HashEntry("field1", "serialized 1"),
                    new HashEntry("field2", "serialized 2")
                })), CommandFlags.None))
            .Do(m => called = true);

        var result = _sut.Set("key", ("field1", data1), ("field2", data2));

        result.IsRight.Should().BeTrue();
        called.Should().BeTrue();
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
            .HashSet("key", "field", "serialized", When.Always, CommandFlags.None)
            .Returns(_ => throw exception);

        var result = _sut.Set("key", "field", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }

    [Test]
    public void MultipleSet_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        var exception = new Exception("some message");
        var data1 = new TestData("id1");
        var data2 = new TestData("id2");

        _mockSerDes
            .Serialize(data1)
            .Returns((RedisValue)"serialized 1");
        _mockSerDes
            .Serialize(data2)
            .Returns((RedisValue)"serialized 2");
        _mockDb
            .When(m => m.HashSet("key", Arg.Is<HashEntry[]>(h => h.SequenceEqual(new[] { new HashEntry("field1", "serialized 1"), new HashEntry("field2", "serialized 2") })), CommandFlags.None))
            .Do(_ => throw exception);

        var result = _sut.Set("key", ("field1", data1), ("field2", data2));

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }
}