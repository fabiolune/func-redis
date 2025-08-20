namespace Func.Redis.Tests.SortedSet.RedisSortedSetService;
internal partial class RedisSortedSetServiceTests
{
    [Test]
    public void RangeByScore_WhenDataArePresent_ShouldReturnCorrectRange()
    {
        _mockDb
            .SortedSetRangeByScore("key", 1, 10)
            .Returns(["1", "2", "3"]);

        _mockSerDes
            .Deserialize<int>("1")
            .Returns(1.ToOption());
        _mockSerDes
            .Deserialize<int>("2")
            .Returns(2.ToOption());
        _mockSerDes
            .Deserialize<int>("3")
            .Returns(Option<int>.None());

        var result = _sut.RangeByScore<int>("key", 1, 10);

        result.IsRight.ShouldBeTrue();
        result.OnRight(v => v.ShouldBe([1, 2]));
    }

    [Test]
    public async Task RangeByScoreAsync_WhenDataArePresent_ShouldReturnCorrectRange()
    {
        _mockDb
            .SortedSetRangeByScoreAsync("key", 1, 10)
            .Returns(["1", "2", "3"]);
        _mockSerDes
            .Deserialize<int>("1")
            .Returns(1.ToOption());
        _mockSerDes
            .Deserialize<int>("2")
            .Returns(2.ToOption());
        _mockSerDes
            .Deserialize<int>("3")
            .Returns(Option<int>.None());

        var result = await _sut.RangeByScoreAsync<int>("key", 1, 10);

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.ShouldBe([1, 2]));
    }

    [Test]
    public void RangeByScore_WhenDeserializerThrows_ShouldReturnError()
    {
        _mockDb
            .SortedSetRangeByScore("key", 1, 10)
            .Returns(["1", "2", "3"]);
        _mockSerDes
            .Deserialize<int>("1")
            .Returns(_ => throw new Exception("Deserialization error"));

        var result = _sut.RangeByScore<int>("key", 1, 10);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBeEquivalentTo(Error.New("Deserialization error")));
    }

    [Test]
    public async Task RangeByScoreAsync_WhenDeserializerThrows_ShouldReturnError()
    {
        _mockDb
            .SortedSetRangeByScoreAsync("key", 1, 10)
            .Returns(["1", "2", "3"]);
        _mockSerDes
            .Deserialize<int>("1")
            .Returns(_ => throw new Exception("Deserialization error"));

        var result = await _sut.RangeByScoreAsync<int>("key", 1, 10);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBeEquivalentTo(Error.New("Deserialization error")));
    }

    [Test]
    public void RangeByScore_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        _mockDb
            .SortedSetRangeByScore("key", 1, 10)
            .Returns(_ => throw exception);

        var result = _sut.RangeByScore<int>("key", 1, 10);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
    }

    [Test]
    public async Task RangeByScoreAsync_WhenDatabaseThrows_ShouldReturnError()
    {
        var exception = new Exception("some message");
        _mockDb
            .SortedSetRangeByScoreAsync("key", 1, 10)
            .Returns<RedisValue[]>(_ => throw exception);

        var result = await _sut.RangeByScoreAsync<int>("key", 1, 10);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
    }
}
