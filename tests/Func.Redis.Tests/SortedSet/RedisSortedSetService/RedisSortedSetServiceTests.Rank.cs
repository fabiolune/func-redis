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
        
        result.IsRight.Should().BeTrue();
        result.OnRight(rank =>
        {
            rank.IsSome.Should().BeTrue();
            rank.OnSome(r => r.Should().Be(2));
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
        
        result.IsRight.Should().BeTrue();
        result.OnRight(rank => rank.IsNone.Should().BeTrue());
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
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }

    [Test]
    public void Rank_WhenSerializerThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns(_ => throw new Exception("Serialization error"));
     
        var result = _sut.Rank("test_key", data);
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Serialization error"));
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
        
        result.IsRight.Should().BeTrue();
        result.OnRight(rank =>
        {
            rank.IsSome.Should().BeTrue();
            rank.OnSome(r => r.Should().Be(2));
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
        
        result.IsRight.Should().BeTrue();
        result.OnRight(rank => rank.IsNone.Should().BeTrue());
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
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }

    [Test]
    public async Task RankAsync_WhenSerializerThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns(_ => throw new Exception("Serialization error"));
     
        var result = await _sut.RankAsync("test_key", data);
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Serialization error"));
    }
}
