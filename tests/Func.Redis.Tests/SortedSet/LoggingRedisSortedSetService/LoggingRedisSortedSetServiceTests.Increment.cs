namespace Func.Redis.Tests.SortedSet.LoggingRedisSortedSetService;
internal partial class LoggingRedisSortedSetServiceTests
{
    [Test]
    public void Increment_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .Increment("some key", "value", 10)
            .Returns(Unit.Default);

        var result = _sut.Increment("some key", "value", 10);

        result.IsRight.ShouldBeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: incrementing item \"value\" in \"some key\" by 10");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task IncrementAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .IncrementAsync("some key", "value", 10)
            .Returns(Unit.Default);

        var result = await _sut.IncrementAsync("some key", "value", 10);

        result.IsRight.ShouldBeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async incrementing item \"value\" in \"some key\" by 10");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void Increment_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("Some error");
        _mockService
            .Increment("some key", "value", 10)
            .Returns(error);

        var result = _sut.Increment("some key", "value", 10);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: incrementing item \"value\" in \"some key\" by 10");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task IncrementAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("Some error");
        _mockService
            .IncrementAsync("some key", "value", 10)
            .Returns(error);

        var result = await _sut.IncrementAsync("some key", "value", 10);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async incrementing item \"value\" in \"some key\" by 10");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}
