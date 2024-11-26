using static Func.Redis.Tests.TestDataElements;

namespace Func.Redis.Tests.RedisListService;
internal partial class RedisListServiceTests
{
    [Test]
    public void Pop_WhenDatabaseThrows_ShouldReturnLeft()
    {
        _mockDb
            .ListRightPop("key")
            .Returns(_ => throw new Exception("some error"));

        var result = _sut.Pop<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public void Pop_WhenDeserializerThrows_ShouldReturnLeft()
    {
        _mockDb
            .ListRightPop("key")
            .Returns((RedisValue)"serialized");

        _mockSerDes
            .Deserialize<object>("serialized")
            .Returns(_ => throw new Exception("some error"));

        var result = _sut.Pop<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public void Pop_WhenDeserializerReturnsNone_ShouldReturnNone()
    {
        _mockDb
            .ListRightPop("key")
            .Returns((RedisValue)"serialized");

        _mockSerDes
            .Deserialize<object>("serialized")
            .Returns(Option<object>.None());

        var result = _sut.Pop<object>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(value => value.IsNone.Should().BeTrue());
    }

    [Test]
    public void Pop_WhenDeserializerReturnsSome_ShouldReturnSome()
    {
        var data = new TestData(27);
        _mockDb
            .ListRightPop("key")
            .Returns((RedisValue)"serialized");

        _mockSerDes
            .Deserialize<TestData>("serialized")
            .Returns(Option<TestData>.Some(data));

        var result = _sut.Pop<TestData>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(value =>
        {
            value.IsSome.Should().BeTrue();
            value.OnSome(d => d.Should().BeEquivalentTo(data));
        });
    }

    [Test]
    public void PopCount_WhenDatabaseThrows_ShouldReturnLeft()
    {
        _mockDb
            .ListRightPop("key", 3)
            .Returns(_ => throw new Exception("some error"));

        var result = _sut.Pop<object>("key", 3);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public void PopCount_WhenDeserializerThrows_ShouldReturnLeft()
    {
        var value1 = new TestData(27);
        var value2 = new TestData(42);

        var returnedData = new[] { (RedisValue)"ser1", (RedisValue)"ser2" };
        _mockDb
            .ListRightPop("key", 3)
            .Returns(returnedData);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser1")
            .Returns(Option<TestData>.Some(value1));

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser2")
            .Returns(_ => throw new Exception("some error"));

        var result = _sut.Pop<TestData>("key", 3);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public void PopCount_WhenDeserializerReturnsNone_ShouldReturnNone()
    {
        var value1 = new TestData(27);
        var expected = new[] { value1 };

        var returnedData = new[] { (RedisValue)"ser1", (RedisValue)"ser2" };
        _mockDb
            .ListRightPop("key", 3)
            .Returns(returnedData);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser1")
            .Returns(Option<TestData>.Some(value1));

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser2")
            .Returns(Option<TestData>.None());

        var result = _sut.Pop<TestData>("key", 3);

        result.IsRight.Should().BeTrue();
        result.OnRight(value =>
        {
            value.Should().HaveCount(2);
            value.Filter().Should().BeEquivalentTo(expected);
        });
    }

    [Test]
    public void PopCount_WhenDeserializerReturnsSome_ShouldReturnSome()
    {
        var value1 = new TestData(27);
        var value2 = new TestData(42);
        var expected = new[] { value1, value2 };

        var returnedData = new[] { (RedisValue)"ser1", (RedisValue)"ser2" };
        _mockDb
            .ListRightPop("key", 3)
            .Returns(returnedData);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser1")
            .Returns(Option<TestData>.Some(value1));

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser2")
            .Returns(Option<TestData>.Some(value2));

        var result = _sut.Pop<TestData>("key", 3);

        result.IsRight.Should().BeTrue();
        result.OnRight(value =>
        {
            value.Should().HaveCount(2);
            value.Filter().Should().BeEquivalentTo(expected);
        });
    }

    [Test]
    public async Task PopAsync_WhenDatabaseThrows_ShouldReturnLeft()
    {
        _mockDb
            .ListRightPopAsync("key")
            .Returns<RedisValue>(_ => throw new Exception("some error"));

        var result = await _sut.PopAsync<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public async Task PopAsync_WhenDeserializerThrows_ShouldReturnLeft()
    {
        _mockDb
            .ListRightPopAsync("key")
            .Returns((RedisValue)"serialized");

        _mockSerDes
            .Deserialize<object>("serialized")
            .Returns(_ => throw new Exception("some error"));

        var result = await _sut.PopAsync<object>("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public async Task PopAsync_WhenDeserializerReturnsNone_ShouldReturnNone()
    {
        _mockDb
            .ListRightPopAsync("key")
            .Returns((RedisValue)"serialized");

        _mockSerDes
            .Deserialize<object>("serialized")
            .Returns(Option<object>.None());

        var result = await _sut.PopAsync<object>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(value => value.IsNone.Should().BeTrue());
    }

    [Test]
    public async Task PopAsync_WhenDeserializerReturnsSome_ShouldReturnSome()
    {
        var data = new TestData(27);
        _mockDb
            .ListRightPopAsync("key")
            .Returns((RedisValue)"serialized");

        _mockSerDes
            .Deserialize<TestData>("serialized")
            .Returns(Option<TestData>.Some(data));

        var result = await _sut.PopAsync<TestData>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(value =>
        {
            value.IsSome.Should().BeTrue();
            value.OnSome(d => d.Should().BeEquivalentTo(data));
        });
    }

    [Test]
    public async Task PopCountAsync_WhenDatabaseThrows_ShouldReturnLeft()
    {
        _mockDb
            .ListRightPopAsync("key", 3)
            .Returns<RedisValue[]>(_ => throw new Exception("some error"));

        var result = await _sut.PopAsync<object>("key", 3);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public async Task PopCountAsync_WhenDeserializerThrows_ShouldReturnLeft()
    {
        var value1 = new TestData(27);
        var value2 = new TestData(42);

        var returnedData = new[] { (RedisValue)"ser1", (RedisValue)"ser2" };
        _mockDb
            .ListRightPopAsync("key", 3)
            .Returns(returnedData);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser1")
            .Returns(Option<TestData>.Some(value1));

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser2")
            .Returns(_ => throw new Exception("some error"));

        var result = await _sut.PopAsync<TestData>("key", 3);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(error => error.Message.Should().Be("some error"));
    }

    [Test]
    public async Task PopCountAsync_WhenDeserializerReturnsNone_ShouldReturnNone()
    {
        var value1 = new TestData(27);
        var expected = new[] { value1 };

        var returnedData = new[] { (RedisValue)"ser1", (RedisValue)"ser2" };
        _mockDb
            .ListRightPopAsync("key", 3)
            .Returns(returnedData);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser1")
            .Returns(Option<TestData>.Some(value1));

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser2")
            .Returns(Option<TestData>.None());

        var result = await _sut.PopAsync<TestData>("key", 3);

        result.IsRight.Should().BeTrue();
        result.OnRight(value =>
        {
            value.Should().HaveCount(2);
            value.Filter().Should().BeEquivalentTo(expected);
        });
    }

    [Test]
    public async Task PopCountAsync_WhenDeserializerReturnsSome_ShouldReturnSome()
    {
        var value1 = new TestData(27);
        var value2 = new TestData(42);
        var expected = new[] { value1, value2 };

        var returnedData = new[] { (RedisValue)"ser1", (RedisValue)"ser2" };
        _mockDb
            .ListRightPopAsync("key", 3)
            .Returns(returnedData);

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser1")
            .Returns(Option<TestData>.Some(value1));

        _mockSerDes
            .Deserialize<TestData>((RedisValue)"ser2")
            .Returns(Option<TestData>.Some(value2));

        var result = await _sut.PopAsync<TestData>("key", 3);

        result.IsRight.Should().BeTrue();
        result.OnRight(value =>
        {
            value.Should().HaveCount(2);
            value.Filter().Should().BeEquivalentTo(expected);
        });
    }
}