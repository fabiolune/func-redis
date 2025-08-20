namespace Func.Redis.Tests.SortedSet.LoggingRedisSortedSetService;
internal partial class LoggingRedisSortedSetServiceTests
{
    [Test]
    public void Score_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var value = new object();
        _mockService
            .Score("key", value)
            .Returns(10.0.ToOption());

        var result = _sut.Score("key", value);

        result.IsRight.ShouldBeTrue();
        result.OnRight(o =>
        {
            o.IsSome.ShouldBeTrue();
            o.OnSome(l => l.ShouldBe(10.0));
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: getting score of item in sorted set at \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task ScoreAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var value = new object();
        _mockService
            .ScoreAsync("key", value)
            .Returns(10.0.ToOption());

        var result = await _sut.ScoreAsync("key", value);

        result.IsRight.ShouldBeTrue();
        result.OnRight(o =>
        {
            o.IsSome.ShouldBeTrue();
            o.OnSome(l => l.ShouldBe(10.0));
        });
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async getting score of item in sorted set at \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void Score_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var value = new object();
        var error = Error.New("some error");
        _mockService
            .Score("key", value)
            .Returns(error);

        var result = _sut.Score("key", value);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: getting score of item in sorted set at \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task ScoreAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var value = new object();
        var error = Error.New("some error");
        _mockService
            .ScoreAsync("key", value)
            .Returns(error);

        var result = await _sut.ScoreAsync("key", value);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async getting score of item in sorted set at \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

}
