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

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: incrementing item \"value\" in \"some key\" by 10");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task IncrementAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .IncrementAsync("some key", "value", 10)
            .Returns(Unit.Default);

        var result = await _sut.IncrementAsync("some key", "value", 10);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async incrementing item \"value\" in \"some key\" by 10");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: incrementing item \"value\" in \"some key\" by 10");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async incrementing item \"value\" in \"some key\" by 10");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}
