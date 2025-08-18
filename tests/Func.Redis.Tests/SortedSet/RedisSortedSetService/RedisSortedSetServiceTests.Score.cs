namespace Func.Redis.Tests.SortedSet.RedisSortedSetService;
internal partial class RedisSortedSetServiceTests
{
    [Test]
    public void Score_WhenDataBaseReturnsValue_ShouldReturnSome()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .SortedSetScore("test_key", "serialized")
            .Returns(10.0);

        var result = _sut.Score("test_key", data);
        
        result.IsRight.Should().BeTrue();
        result.OnRight(score =>
        {
            score.IsSome.Should().BeTrue();
            score.OnSome(s => s.Should().Be(10.0));
        });
    }

    [Test]
    public void Score_WhenDataBaseReturnsNull_ShouldReturnNone()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .SortedSetScore("test_key", "serialized")
            .Returns((double?)null);

        var result = _sut.Score("test_key", data);
        
        result.IsRight.Should().BeTrue();
        result.OnRight(score => score.IsNone.Should().BeTrue());
    }

    [Test]
    public void Score_WhenDataBaseThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .SortedSetScore("test_key", "serialized")
            .Returns(_ => throw new Exception("Redis error"));
     
        var result = _sut.Score("test_key", data);
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }

    [Test]
    public void Score_WhenSerializerThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns(_ => throw new Exception("Serialization error"));
        var result = _sut.Score("test_key", data);
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Serialization error"));
    }

    [Test]
    public async Task ScoreAsync_WhenDataBaseReturnsValue_ShouldReturnSome()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .SortedSetScoreAsync("test_key", "serialized")
            .Returns(10.0);

        var result = await _sut.ScoreAsync("test_key", data);
        
        result.IsRight.Should().BeTrue();
        result.OnRight(score =>
        {
            score.IsSome.Should().BeTrue();
            score.OnSome(s => s.Should().Be(10.0));
        });
    }

    [Test]
    public async Task ScoreAsync_WhenDataBaseReturnsNull_ShouldReturnNone()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .SortedSetScoreAsync("test_key", "serialized")
            .Returns((double?)null);
     
        var result = await _sut.ScoreAsync("test_key", data);
        
        result.IsRight.Should().BeTrue();
        result.OnRight(score => score.IsNone.Should().BeTrue());
    }

    [Test]
    public async Task ScoreAsync_WhenDataBaseThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns((RedisValue)"serialized");
        _mockDb
            .SortedSetScoreAsync("test_key", "serialized")
            .Returns<double?>(_ => throw new Exception("Redis error"));
     
        var result = await _sut.ScoreAsync("test_key", data);
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Redis error"));
    }

    [Test]
    public async Task ScoreAsync_WhenSerializerThrowsException_ShouldReturnError()
    {
        var data = new TestData(1);
        _mockSerDes
            .Serialize(data)
            .Returns(_ => throw new Exception("Serialization error"));
        
        var result = await _sut.ScoreAsync("test_key", data);
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Message.Should().Be("Serialization error"));
    }
}
