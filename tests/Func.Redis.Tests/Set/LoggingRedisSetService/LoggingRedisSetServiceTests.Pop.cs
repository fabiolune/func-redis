namespace Func.Redis.Tests.LoggingRedisSetService;
internal partial class LoggingRedisSetServiceTests
{
    [Test]
    public void Pop_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object().ToOption();
        _mockService
            .Pop<object>("key")
            .Returns(data);

        var result = _sut.Pop<object>("key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.ShouldBe(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: popping item from \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void Pop_WhenServiceReturnsNone_ShouldReturnNone()
    {
        var data = Option<object>.None();
        _mockService
            .Pop<object>("key")
            .Returns(data);

        var result = _sut.Pop<object>("key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.ShouldBe(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: popping item from \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void Pop_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .Pop<object>("key")
            .Returns(error);

        var result = _sut.Pop<object>("key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: popping item from \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task PopAsync_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object().ToOption();
        _mockService
            .PopAsync<object>("key")
            .Returns(data);

        var result = await _sut.PopAsync<object>("key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.ShouldBe(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: async popping item from \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task PopAsync_WhenServiceReturnsNone_ShouldReturnNone()
    {
        var data = Option<object>.None();
        _mockService
            .PopAsync<object>("key")
            .Returns(data);

        var result = await _sut.PopAsync<object>("key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.ShouldBe(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: async popping item from \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task PopAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .PopAsync<object>("key")
            .Returns(error);

        var result = await _sut.PopAsync<object>("key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: async popping item from \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}
