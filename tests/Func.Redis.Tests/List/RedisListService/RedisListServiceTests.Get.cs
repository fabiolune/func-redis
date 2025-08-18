namespace Func.Redis.Tests.RedisListService;

internal partial class RedisListServiceTests
{
    [Test]
    public void Get_WhenDatabaseReturnsValue_ShouldReturnRight()
    {
        var data = new TestData(1);
        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized")
            .Returns(data.ToOption());
        _mockDb
            .ListGetByIndex("key", 0)
            .Returns((RedisValue)"serialized");

        var result = _sut.Get<TestData>("key", 0);

        result.IsRight.Should().BeTrue();
        result.OnRight(o =>
        {
            o.IsSome.Should().BeTrue();
            o.OnSome(d => d.Should().BeEquivalentTo(data));
        });
    }

    [Test]
    public async Task GetAsync_WhenDatabaseReturnsValue_ShouldReturnRight()
    {
        var data = new TestData(1);
        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized")
            .Returns(data.ToOption());
        _mockDb
            .ListGetByIndexAsync("key", 0)
            .Returns((RedisValue)"serialized");

        var result = await _sut.GetAsync<TestData>("key", 0);

        result.IsRight.Should().BeTrue();
        result.OnRight(o =>
        {
            o.IsSome.Should().BeTrue();
            o.OnSome(d => d.Should().BeEquivalentTo(data));
        });
    }

    [Test]
    public void Get_WhenDatabaseThrowsException_ShouldReturnLeft()
    {
        _mockDb
            .ListGetByIndex("key", 0)
            .Returns(_ => throw new Exception("Redis Exception"));

        var result = _sut.Get<TestData>("key", 0);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public async Task GetAsync_WhenDatabaseThrowsException_ShouldReturnLeft()
    {
        _mockDb
            .ListGetByIndexAsync("key", 0)
            .Returns<RedisValue>(_ => throw new Exception("Redis Exception"));

        var result = await _sut.GetAsync<TestData>("key", 0);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public void Get_WhenSerializer_ReturnsNone_ShouldReturnRight()
    {
        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized")
            .Returns(Option<TestData>.None());
        _mockDb
            .ListGetByIndex("key", 0)
            .Returns((RedisValue)"serialized");

        var result = _sut.Get<TestData>("key", 0);

        result.IsRight.Should().BeTrue();
        result.OnRight(o => o.IsNone.Should().BeTrue());
    }

    [Test]
    public async Task GetAsync_WhenSerializer_ReturnsNone_ShouldReturnRight()
    {
        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized")
            .Returns(Option<TestData>.None());
        _mockDb
            .ListGetByIndexAsync("key", 0)
            .Returns((RedisValue)"serialized");

        var result = await _sut.GetAsync<TestData>("key", 0);

        result.IsRight.Should().BeTrue();
        result.OnRight(o => o.IsNone.Should().BeTrue());
    }

    [Test]
    public void Get_WhenDatabaseReturnsValues_ShouldReturnRight()
    {
        var value1 = new TestData(1);
        var value2 = new TestData(2);
        var data = new[] { value1, value2 };
        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized1")
            .Returns(value1.ToOption());
        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized2")
            .Returns(value2.ToOption());
        _mockDb
            .ListRange("key", 0, 1)
            .Returns([(RedisValue)"serialized1", (RedisValue)"serialized2"]);

        var result = _sut.Get<TestData>("key", 0, 1);

        result.IsRight.Should().BeTrue();
        result.OnRight(o => o.Filter().Should().BeEquivalentTo(data));
    }

    [Test]
    public async Task GetAsync_WhenDatabaseReturnsValues_ShouldReturnRight()
    {
        var value1 = new TestData(1);
        var value2 = new TestData(2);
        var data = new[] { value1, value2 };
        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized1")
            .Returns(value1.ToOption());
        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized2")
            .Returns(value2.ToOption());
        _mockDb
            .ListRangeAsync("key", 0, 1)
            .Returns([(RedisValue)"serialized1", (RedisValue)"serialized2"]);

        var result = await _sut.GetAsync<TestData>("key", 0, 1);

        result.IsRight.Should().BeTrue();
        result.OnRight(o => o.Filter().Should().BeEquivalentTo(data));
    }

    [Test]
    public void Get_WhenSerializerThrowsException_ShouldReturnLeft()
    {
        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized")
            .Returns(_ => throw new Exception("serdes exception"));
        _mockDb
            .ListRange("key", 0, 1)
            .Returns([(RedisValue)"serialized"]);

        var result = _sut.Get<TestData>("key", 0, 1);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("serdes exception")));
    }

    [Test]
    public async Task GetAsync_WhenSerializerThrowsException_ShouldReturnLeft()
    {
        _mockSerDes
            .Deserialize<TestData>((RedisValue)"serialized")
            .Returns(_ => throw new Exception("serdes exception"));
        _mockDb
            .ListRangeAsync("key", 0, 1)
            .Returns([(RedisValue)"serialized"]);

        var result = await _sut.GetAsync<TestData>("key", 0, 1);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("serdes exception")));
    }

    [Test]
    public void MultipleGet_WhenDatabaseThrowsException_ShouldReturnLeft()
    {
        _mockDb
            .ListRange("key", 0, 1)
            .Returns(_ => throw new Exception("Redis Exception"));

        var result = _sut.Get<TestData>("key", 0, 1);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }

    [Test]
    public async Task MultipleGetAsync_WhenDatabaseThrowsException_ShouldReturnLeft()
    {
        _mockDb
            .ListRangeAsync("key", 0, 1)
            .Returns<RedisValue[]>(_ => throw new Exception("Redis Exception"));

        var result = await _sut.GetAsync<TestData>("key", 0, 1);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Redis Exception")));
    }
}
