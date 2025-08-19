namespace Func.Redis.Tests.SortedSet.LoggingRedisSortedSetService;
internal partial class LoggingRedisSortedSetServiceTests
{
    [Test]
    public void RangeByScore_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .RangeByScore<int>("some key", 1, 10)
            .Returns(Either<Error, int[]>.Right([1, 2]));

        var result = _sut.RangeByScore<int>("some key", 1, 10);

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.Should().BeEquivalentTo([1, 2]));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: getting range by score from sorted set at \"some key\" with range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task RangeByScoreAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .RangeByScoreAsync<int>("some key", 1, 10)
            .Returns(Either<Error, int[]>.Right([1, 2]));

        var result = await _sut.RangeByScoreAsync<int>("some key", 1, 10);

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.Should().BeEquivalentTo([1, 2]));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async getting range by score from sorted set at \"some key\" with range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public void RangeByScore_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("Some error");
        _mockService
            .RangeByScore<int>("some key", 1, 10)
            .Returns(Either<Error, int[]>.Left(error));

        var result = _sut.RangeByScore<int>("some key", 1, 10);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: getting range by score from sorted set at \"some key\" with range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task RangeByScoreAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("Some error");
        _mockService
            .RangeByScoreAsync<int>("some key", 1, 10)
            .Returns(Either<Error, int[]>.Left(error));

        var result = await _sut.RangeByScoreAsync<int>("some key", 1, 10);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async getting range by score from sorted set at \"some key\" with range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}
