using Func.Redis.SortedSet;

namespace Func.Redis.Tests.SortedSet;
internal class KeyTransformerRedisSortedSetServiceTests
{
    private KeyTransformerRedisSortedSetService _sut;
    private IRedisSortedSetService _mockService;

    [SetUp]
    public void SetUp()
    {
        _mockService = Substitute.For<IRedisSortedSetService>();
        _sut = new KeyTransformerRedisSortedSetService(_mockService, k => $"mapped_{k}");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void Add_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.Add("mapped_key", "value", 5L).Returns(internalResult);

        var result = _sut.Add(key, "value", 5L);

        result.Should().Be(internalResult);
        _mockService.Received(1).Add("mapped_key", "value", 5L);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void AddMultiple_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var data = new[] { ("value1", 1.0), ("value2", 2.0) };
        _mockService.Add("mapped_key", data).Returns(internalResult);

        var result = _sut.Add(key, data);

        result.Should().Be(internalResult);
        _mockService.Received(1).Add("mapped_key", data);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task AddAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.AddAsync("mapped_key", "value", 5L).Returns(Task.FromResult(internalResult));

        var result = await _sut.AddAsync(key, "value", 5L);

        result.Should().Be(internalResult);
        await _mockService.Received(1).AddAsync("mapped_key", "value", 5L);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task AddMultipleAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var data = new[] { ("value1", 1.0), ("value2", 2.0) };
        _mockService.AddAsync("mapped_key", data).Returns(Task.FromResult(internalResult));

        var result = await _sut.AddAsync(key, data);

        result.Should().Be(internalResult);
        await _mockService.Received(1).AddAsync("mapped_key", data);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void Decrement_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.Decrement("mapped_key", "value", 5L).Returns(internalResult);

        var result = _sut.Decrement(key, "value", 5L);

        result.Should().Be(internalResult);
        _mockService.Received(1).Decrement("mapped_key", "value", 5L);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task DecrementAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.DecrementAsync("mapped_key", "value", 5L).Returns(Task.FromResult(internalResult));

        var result = await _sut.DecrementAsync(key, "value", 5L);

        result.Should().Be(internalResult);
        await _mockService.Received(1).DecrementAsync("mapped_key", "value", 5L);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void Increment_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.Increment("mapped_key", "value", 5L).Returns(internalResult);

        var result = _sut.Increment(key, "value", 5L);

        result.Should().Be(internalResult);
        _mockService.Received(1).Increment("mapped_key", "value", 5L);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task IncrementAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.IncrementAsync("mapped_key", "value", 5L).Returns(Task.FromResult(internalResult));

        var result = await _sut.IncrementAsync(key, "value", 5L);

        result.Should().Be(internalResult);
        await _mockService.Received(1).IncrementAsync("mapped_key", "value", 5L);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorLongTestData))]
    public void Length_ShouldCallServiceWithMappedKey(Either<Error, long> internalResult)
    {
        var key = "key";
        _mockService.Length("mapped_key").Returns(internalResult);

        var result = _sut.Length(key);

        result.Should().Be(internalResult);
        _mockService.Received(1).Length("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorLongTestData))]
    public async Task LengthAsync_ShouldCallServiceWithMappedKey(Either<Error, long> internalResult)
    {
        var key = "key";
        _mockService.LengthAsync("mapped_key").Returns(Task.FromResult(internalResult));

        var result = await _sut.LengthAsync(key);

        result.Should().Be(internalResult);
        await _mockService.Received(1).LengthAsync("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorLongTestData))]
    public void LengthByScore_ShouldCallServiceWithMappedKey(Either<Error, long> internalResult)
    {
        var key = "key";
        _mockService.LengthByScore("mapped_key", 1.0, 10.0).Returns(internalResult);

        var result = _sut.LengthByScore(key, 1.0, 10.0);

        result.Should().Be(internalResult);
        _mockService.Received(1).LengthByScore("mapped_key", 1.0, 10.0);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorLongTestData))]
    public async Task LengthByScoreAsync_ShouldCallServiceWithMappedKey(Either<Error, long> internalResult)
    {
        var key = "key";
        _mockService.LengthByScoreAsync("mapped_key", 1.0, 10.0).Returns(Task.FromResult(internalResult));

        var result = await _sut.LengthByScoreAsync(key, 1.0, 10.0);

        result.Should().Be(internalResult);
        await _mockService.Received(1).LengthByScoreAsync("mapped_key", 1.0, 10.0);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorLongTestData))]
    public void LengthByValue_ShouldCallServiceWithMappedKey(Either<Error, long> internalResult)
    {
        var key = "key";
        _mockService.LengthByValue("mapped_key", "min", "max").Returns(internalResult);

        var result = _sut.LengthByValue(key, "min", "max");

        result.Should().Be(internalResult);
        _mockService.Received(1).LengthByValue("mapped_key", "min", "max");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorLongTestData))]
    public async Task LengthByValueAsync_ShouldCallServiceWithMappedKey(Either<Error, long> internalResult)
    {
        var key = "key";
        _mockService.LengthByValueAsync("mapped_key", "min", "max").Returns(Task.FromResult(internalResult));

        var result = await _sut.LengthByValueAsync(key, "min", "max");

        result.Should().Be(internalResult);
        await _mockService.Received(1).LengthByValueAsync("mapped_key", "min", "max");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void Remove_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.Remove("mapped_key", "value").Returns(internalResult);

        var result = _sut.Remove(key, "value");

        result.Should().Be(internalResult);
        _mockService.Received(1).Remove("mapped_key", "value");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void RemoveMultiple_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var data = new[] { "value1", "value2" };
        _mockService.Remove<string>("mapped_key", data).Returns(internalResult);

        var result = _sut.Remove<string>(key, data);

        result.Should().Be(internalResult);
        _mockService.Received(1).Remove<string>("mapped_key", data);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task RemoveAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.RemoveAsync("mapped_key", "value").Returns(internalResult);

        var result = await _sut.RemoveAsync(key, "value");

        result.Should().Be(internalResult);
        await _mockService.Received(1).RemoveAsync("mapped_key", "value");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task RemoveMultipleAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var data = new[] { "value1", "value2" };
        _mockService.RemoveAsync<string>("mapped_key", data).Returns(internalResult);

        var result = await _sut.RemoveAsync<string>(key, data);

        result.Should().Be(internalResult);
        await _mockService.Received(1).RemoveAsync<string>("mapped_key", data);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void RemoveRangeByScore_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.RemoveRangeByScore("mapped_key", 1.0, 10.0).Returns(internalResult);

        var result = _sut.RemoveRangeByScore(key, 1.0, 10.0);

        result.Should().Be(internalResult);
        _mockService.Received(1).RemoveRangeByScore("mapped_key", 1.0, 10.0);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task RemoveRangeByScoreAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.RemoveRangeByScoreAsync("mapped_key", 1.0, 10.0).Returns(Task.FromResult(internalResult));

        var result = await _sut.RemoveRangeByScoreAsync(key, 1.0, 10.0);

        result.Should().Be(internalResult);
        await _mockService.Received(1).RemoveRangeByScoreAsync("mapped_key", 1.0, 10.0);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void RemoveRangeByValue_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.RemoveRangeByValue("mapped_key", "min", "max").Returns(internalResult);

        var result = _sut.RemoveRangeByValue(key, "min", "max");

        result.Should().Be(internalResult);
        _mockService.Received(1).RemoveRangeByValue("mapped_key", "min", "max");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task RemoveRangeByValueAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.RemoveRangeByValueAsync("mapped_key", "min", "max").Returns(Task.FromResult(internalResult));

        var result = await _sut.RemoveRangeByValueAsync(key, "min", "max");

        result.Should().Be(internalResult);
        await _mockService.Received(1).RemoveRangeByValueAsync("mapped_key", "min", "max");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringsTestData))]
    public void Intersect_ShouldCallServiceWithMappedKeys(Either<Error, string[]> internalResult)
    {
        var keys = new[] { "key1", "key2" };
        var mappedKeys = new[] { "mapped_key1", "mapped_key2" };

        _mockService.Intersect<string>(Arg.Is<string[]>(a => a.SequenceEqual(mappedKeys))).Returns(internalResult);

        var result = _sut.Intersect<string>(keys);

        result.Should().Be(internalResult);
        _mockService.Received(1).Intersect<string>(Arg.Is<string[]>(a => a.SequenceEqual(mappedKeys)));
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringsTestData))]
    public async Task IntersectAsync_ShouldCallServiceWithMappedKeys(Either<Error, string[]> internalResult)
    {
        var keys = new[] { "key1", "key2" };
        var mappedKeys = new[] { "mapped_key1", "mapped_key2" };
        _mockService.IntersectAsync<string>(Arg.Is<string[]>(a => a.SequenceEqual(mappedKeys))).Returns(Task.FromResult(internalResult));

        var result = await _sut.IntersectAsync<string>(keys);

        result.Should().Be(internalResult);
        await _mockService.Received(1).IntersectAsync<string>(Arg.Is<string[]>(a => a.SequenceEqual(mappedKeys)));
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringsTestData))]
    public void Union_ShouldCallServiceWithMappedKeys(Either<Error, string[]> internalResult)
    {
        var keys = new[] { "key1", "key2" };
        var mappedKeys = new[] { "mapped_key1", "mapped_key2" };

        _mockService.Union<string>(Arg.Is<string[]>(a => a.SequenceEqual(mappedKeys))).Returns(internalResult);

        var result = _sut.Union<string>(keys);

        result.Should().Be(internalResult);
        _mockService.Received(1).Union<string>(Arg.Is<string[]>(a => a.SequenceEqual(mappedKeys)));
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringsTestData))]
    public async Task UnionAsync_ShouldCallServiceWithMappedKeys(Either<Error, string[]> internalResult)
    {
        var keys = new[] { "key1", "key2" };
        var mappedKeys = new[] { "mapped_key1", "mapped_key2" };
        _mockService.UnionAsync<string>(Arg.Is<string[]>(a => a.SequenceEqual(mappedKeys))).Returns(Task.FromResult(internalResult));

        var result = await _sut.UnionAsync<string>(keys);

        result.Should().Be(internalResult);
        await _mockService.Received(1).UnionAsync<string>(Arg.Is<string[]>(a => a.SequenceEqual(mappedKeys)));
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorOptionLongTestData))]
    public void Rank_ShouldCallServiceWithMappedKey(Either<Error, Option<long>> internalResult)
    {
        var key = "key";
        _mockService.Rank("mapped_key", "value").Returns(internalResult);

        var result = _sut.Rank(key, "value");

        result.Should().Be(internalResult);
        _mockService.Received(1).Rank("mapped_key", "value");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorOptionLongTestData))]
    public async Task RankAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<long>> internalResult)
    {
        var key = "key";
        _mockService.RankAsync("mapped_key", "value").Returns(internalResult);

        var result = await _sut.RankAsync(key, "value");

        result.Should().Be(internalResult);
        await _mockService.Received(1).RankAsync("mapped_key", "value");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorOptionDoubleTestData))]
    public void Score_ShouldCallServiceWithMappedKey(Either<Error, Option<double>> internalResult)
    {
        var key = "key";
        _mockService.Score("mapped_key", "value").Returns(internalResult);

        var result = _sut.Score(key, "value");

        result.Should().Be(internalResult);
        _mockService.Received(1).Score("mapped_key", "value");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorOptionDoubleTestData))]
    public async Task ScoreAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<double>> internalResult)
    {
        var key = "key";
        _mockService.ScoreAsync("mapped_key", "value").Returns(internalResult);

        var result = await _sut.ScoreAsync(key, "value");

        result.Should().Be(internalResult);
        await _mockService.Received(1).ScoreAsync("mapped_key", "value");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringsTestData))]
    public void RangeByScore_ShouldCallServiceWithMappedKey(Either<Error, string[]> internalResult)
    {
        var key = "key";
        _mockService.RangeByScore<string>("mapped_key", 1.0, 10.0).Returns(internalResult);

        var result = _sut.RangeByScore<string>(key, 1.0, 10.0);

        result.Should().Be(internalResult);
        _mockService.Received(1).RangeByScore<string>("mapped_key", 1.0, 10.0);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringsTestData))]
    public async Task RangeByScoreAsync_ShouldCallServiceWithMappedKey(Either<Error, string[]> internalResult)
    {
        var key = "key";
        _mockService
            .RangeByScoreAsync<string>("mapped_key", 1.0, 10.0)
            .Returns(internalResult);

        var result = await _sut.RangeByScoreAsync<string>(key, 1.0, 10.0);

        result.Should().Be(internalResult);
        await _mockService.Received(1).RangeByScoreAsync<string>("mapped_key", 1.0, 10.0);
    }
}