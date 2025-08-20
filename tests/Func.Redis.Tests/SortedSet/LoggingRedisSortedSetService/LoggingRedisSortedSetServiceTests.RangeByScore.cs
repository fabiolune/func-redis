using TinyFp.Extensions;

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

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.ShouldBe([1, 2]));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: getting range by score from sorted set at \"some key\" with range [1, 10]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task RangeByScoreAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .RangeByScoreAsync<int>("some key", 1, 10)
            .Returns(Either<Error, int[]>.Right([1, 2]));

        var result = await _sut.RangeByScoreAsync<int>("some key", 1, 10);

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.ShouldBe([1, 2]));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async getting range by score from sorted set at \"some key\" with range [1, 10]");
            e.LogLevel.ShouldBe(LogLevel.Information);
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

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: getting range by score from sorted set at \"some key\" with range [1, 10]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
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

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async getting range by score from sorted set at \"some key\" with range [1, 10]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}
