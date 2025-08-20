namespace Func.Redis.Tests.LoggingRedisListService;
internal partial class LoggingRedisServiceListTests
{
    [Test]
    public void Size_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = 42;
        _mockService
            .Size("some key")
            .Returns(data);

        var result = _sut.Size("some key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.ShouldBe(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: getting size for \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void Size_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .Size("some key")
            .Returns(error);

        var result = _sut.Size("some key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: getting size for \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task SizeAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = 42;
        _mockService
            .SizeAsync("some key")
            .Returns(data);

        var result = await _sut.SizeAsync("some key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.ShouldBe(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: async getting size for \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task SizeAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .SizeAsync("some key")
            .Returns(error);

        var result = await _sut.SizeAsync("some key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: async getting size for \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}
