using static Func.Redis.Tests.TestDataElements;

namespace Func.Redis.Tests.RedisListService;
internal partial class RedisListServiceTests
{
    [Test]
    public void Shift_WhenDatabaseThrows_ShouldReturnLeft()
    {
        _mockDb
            .ListLeftPop("key")
            .Returns(_ => throw new Exception("some error"));

        var result = _sut.Shift<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public void Shift_WhenDeserializerThrows_ShouldReturnLeft()
    {
        _mockDb
            .ListLeftPop("key")
            .Returns((RedisValue)"serialized");

        _mockSerDes
            .Deserialize<object>("serialized")
            .Returns(_ => throw new Exception("some error"));

        var result = _sut.Shift<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public void Shift_WhenDeserializerReturnsNone_ShouldReturnNone()
    {
        _mockDb
            .ListLeftPop("key")
            .Returns((RedisValue)"serialized");

        _mockSerDes
            .Deserialize<object>("serialized")
            .Returns(Option<object>.None());

        var result = _sut.Shift<object>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(value => value.IsNone.Should().BeTrue());
    }

    [Test]
    public void Shift_WhenDeserializerReturnsSome_ShouldReturnSome()
    {
        var data = new TestData(27);
        _mockDb
            .ListLeftPop("key")
            .Returns((RedisValue)"serialized");

        _mockSerDes
            .Deserialize<TestData>("serialized")
            .Returns(Option<TestData>.Some(data));

        var result = _sut.Shift<TestData>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(value =>
        {
            value.IsSome.Should().BeTrue();
            value.OnSome(d => d.Should().BeEquivalentTo(data));
        });
    }

    [Test]
    public void ShiftCount_WhenDatabaseThrows_ShouldReturnLeft()
    {
        _mockDb
            .ListLeftPop("key", 3)
            .Returns(_ => throw new Exception("some error"));

        var result = _sut.Shift<object>("key", 3);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public void ShiftCount_WhenDeserializerThrows_ShouldReturnLeft()
    {
        var value1 = new TestData(27);
        var value2 = new TestData(42);

        var returnedData = new[] { (RedisValue)"ser1", (RedisValue)"ser2" };
        _mockDb
            .ListLeftPop("key", 3)
            .Returns(returnedData);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser1")
            .Returns(Option<TestData>.Some(value1));

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser2")
            .Returns(_ => throw new Exception("some error"));

        var result = _sut.Shift<TestData>("key", 3);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public void ShiftCount_WhenDeserializerReturnsNone_ShouldReturnNone()
    {
        var value1 = new TestData(27);
        var expected = new[] { value1 };

        var returnedData = new[] { (RedisValue)"ser1", (RedisValue)"ser2" };
        _mockDb
            .ListLeftPop("key", 3)
            .Returns(returnedData);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser1")
            .Returns(Option<TestData>.Some(value1));

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser2")
            .Returns(Option<TestData>.None());

        var result = _sut.Shift<TestData>("key", 3);

        result.IsRight.Should().BeTrue();
        result.OnRight(value =>
        {
            value.Should().HaveCount(2);
            value.Filter().Should().BeEquivalentTo(expected);
        });
    }

    [Test]
    public void ShiftCount_WhenDeserializerReturnsSome_ShouldReturnSome()
    {
        var value1 = new TestData(27);
        var value2 = new TestData(42);
        var expected = new[] { value1, value2 };

        var returnedData = new[] { (RedisValue)"ser1", (RedisValue)"ser2" };
        _mockDb
            .ListLeftPop("key", 3)
            .Returns(returnedData);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser1")
            .Returns(Option<TestData>.Some(value1));

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser2")
            .Returns(Option<TestData>.Some(value2));

        var result = _sut.Shift<TestData>("key", 3);

        result.IsRight.Should().BeTrue();
        result.OnRight(value =>
        {
            value.Should().HaveCount(2);
            value.Filter().Should().BeEquivalentTo(expected);
        });
    }

    [Test]
    public async Task ShiftAsync_WhenDatabaseThrows_ShouldReturnLeft()
    {
        _mockDb
            .ListLeftPopAsync("key")
            .Returns<RedisValue>(_ => throw new Exception("some error"));

        var result = await _sut.ShiftAsync<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public async Task ShiftAsync_WhenDeserializerThrows_ShouldReturnLeft()
    {
        _mockDb
            .ListLeftPopAsync("key")
            .Returns((RedisValue)"serialized");

        _mockSerDes
            .Deserialize<object>("serialized")
            .Returns(_ => throw new Exception("some error"));

        var result = await _sut.ShiftAsync<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public async Task ShiftAsync_WhenDeserializerReturnsNone_ShouldReturnNone()
    {
        _mockDb
            .ListLeftPopAsync("key")
            .Returns((RedisValue)"serialized");

        _mockSerDes
            .Deserialize<object>("serialized")
            .Returns(Option<object>.None());

        var result = await _sut.ShiftAsync<object>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(value => value.IsNone.Should().BeTrue());
    }

    [Test]
    public async Task ShiftAsync_WhenDeserializerReturnsSome_ShouldReturnSome()
    {
        var data = new TestData(27);
        _mockDb
            .ListLeftPopAsync("key")
            .Returns((RedisValue)"serialized");

        _mockSerDes
            .Deserialize<TestData>("serialized")
            .Returns(Option<TestData>.Some(data));

        var result = await _sut.ShiftAsync<TestData>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(value =>
        {
            value.IsSome.Should().BeTrue();
            value.OnSome(d => d.Should().BeEquivalentTo(data));
        });
    }

    [Test]
    public async Task ShiftCountAsync_WhenDatabaseThrows_ShouldReturnLeft()
    {
        _mockDb
            .ListLeftPopAsync("key", 3)
            .Returns<RedisValue[]>(_ => throw new Exception("some error"));

        var result = await _sut.ShiftAsync<object>("key", 3);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public async Task ShiftCountAsync_WhenDeserializerThrows_ShouldReturnLeft()
    {
        var value1 = new TestData(27);

        var returnedData = new[] { (RedisValue)"ser1", (RedisValue)"ser2" };
        _mockDb
            .ListLeftPopAsync("key", 3)
            .Returns(returnedData);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser1")
            .Returns(Option<TestData>.Some(value1));

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser2")
            .Returns(_ => throw new Exception("some error"));

        var result = await _sut.ShiftAsync<TestData>("key", 3);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public async Task ShiftCountAsync_WhenDeserializerReturnsNone_ShouldReturnNone()
    {
        var value1 = new TestData(27);
        var expected = new[] { value1 };

        var returnedData = new[] { (RedisValue)"ser1", (RedisValue)"ser2" };
        _mockDb
            .ListLeftPopAsync("key", 3)
            .Returns(returnedData);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser1")
            .Returns(Option<TestData>.Some(value1));

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser2")
            .Returns(Option<TestData>.None());

        var result = await _sut.ShiftAsync<TestData>("key", 3);

        result.IsRight.Should().BeTrue();
        result.OnRight(value =>
        {
            value.Should().HaveCount(2);
            value.Filter().Should().BeEquivalentTo(expected);
        });
    }

    [Test]
    public async Task ShiftCountAsync_WhenDeserializerReturnsSome_ShouldReturnSome()
    {
        var value1 = new TestData(27);
        var value2 = new TestData(42);
        var expected = new[] { value1, value2 };

        var returnedData = new[] { (RedisValue)"ser1", (RedisValue)"ser2" };
        _mockDb
            .ListLeftPopAsync("key", 3)
            .Returns(returnedData);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser1")
            .Returns(Option<TestData>.Some(value1));

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser2")
            .Returns(Option<TestData>.Some(value2));

        var result = await _sut.ShiftAsync<TestData>("key", 3);

        result.IsRight.Should().BeTrue();
        result.OnRight(value =>
        {
            value.Should().HaveCount(2);
            value.Filter().Should().BeEquivalentTo(expected);
        });
    }
}