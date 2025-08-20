namespace Func.Redis.Tests.SortedSet.RedisSortedSetService;
internal partial class RedisSortedSetServiceTests
{
    [Test]
    public void Rank_WhenDataBaseReturnsValue_ShouldReturnSome()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");
        _mockDb
            .SortedSetRank("test_key", "serialized_data")
            .Returns(2);

        var result = _sut.Rank("test_key", data);

        result.IsRight.ShouldBeTrue();
        result.OnRight(rank =>
        {
            rank.IsSome.ShouldBeTrue();
            rank.OnSome(r => r.ShouldBe(2));
        });
    }

    [Test]
    public void Rank_WhenDataBaseReturnsNull_ShouldReturnNone()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");
        _mockDb
            .SortedSetRank("test_key", "serialized_data")
            .Returns((long?)null);

        var result = _sut.Rank("test_key", data);

        result.IsRight.ShouldBeTrue();
        result.OnRight(rank => rank.IsNone.ShouldBeTrue());
    }

    [Test]
    public void Rank_WhenDataBaseThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");
        _mockDb
            .SortedSetRank("test_key", "serialized_data")
            .Returns(_ => throw new Exception("Redis error"));

        var result = _sut.Rank("test_key", data);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.Message.ShouldBe("Redis error"));
    }

    [Test]
    public void Rank_WhenSerializerThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns(_ => throw new Exception("Serialization error"));

        var result = _sut.Rank("test_key", data);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.Message.ShouldBe("Serialization error"));
    }

    [Test]
    public async Task RankAsync_WhenDataBaseReturnsValue_ShouldReturnSome()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");
        _mockDb
            .SortedSetRankAsync("test_key", "serialized_data")
            .Returns(Task.FromResult<long?>(2));

        var result = await _sut.RankAsync("test_key", data);

        result.IsRight.ShouldBeTrue();
        result.OnRight(rank =>
        {
            rank.IsSome.ShouldBeTrue();
            rank.OnSome(r => r.ShouldBe(2));
        });
    }

    [Test]
    public async Task RankAsync_WhenDataBaseReturnsNull_ShouldReturnNone()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");
        _mockDb
            .SortedSetRankAsync("test_key", "serialized_data")
            .Returns<long?>((long?)null);

        var result = await _sut.RankAsync("test_key", data);

        result.IsRight.ShouldBeTrue();
        result.OnRight(rank => rank.IsNone.ShouldBeTrue());
    }

    [Test]
    public async Task RankAsync_WhenDataBaseThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized_data");
        _mockDb
            .SortedSetRankAsync("test_key", "serialized_data")
            .Returns<long?>(_ => throw new Exception("Redis error"));

        var result = await _sut.RankAsync("test_key", data);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.Message.ShouldBe("Redis error"));
    }

    [Test]
    public async Task RankAsync_WhenSerializerThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns(_ => throw new Exception("Serialization error"));

        var result = await _sut.RankAsync("test_key", data);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.Message.ShouldBe("Serialization error"));
    }
}
