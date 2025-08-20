namespace Func.Redis.Tests.LoggingRedisListService;
internal partial class LoggingRedisServiceListTests
{
    [Test]
    public void Pop_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object();

        _mockService
            .Pop<object>("some key")
            .Returns(data.ToOption());

        var result = _sut.Pop<object>("some key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(e =>
        {
            e.IsSome.ShouldBeTrue();
            e.OnSome(d => d.ShouldBe(data));
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: popping item from \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void Pop_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");

        _mockService
            .Pop<object>("some key")
            .Returns(error);

        var result = _sut.Pop<object>("some key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: popping item from \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task PopAsync_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object();

        _mockService
            .PopAsync<object>("some key")
            .Returns(data.ToOption());

        var result = await _sut.PopAsync<object>("some key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(e =>
        {
            e.IsSome.ShouldBeTrue();
            e.OnSome(d => d.ShouldBe(data));
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: async popping item from \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task PopAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");

        _mockService
            .PopAsync<object>("some key")
            .Returns(error);

        var result = await _sut.PopAsync<object>("some key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: async popping item from \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public void PopCount_WhenServiceReturnsData_ShouldReturnData()
    {
        var data = new[] { Option<object>.Some(new object()), Option<object>.None() };

        _mockService
            .Pop<object>("key", 3)
            .Returns(data);

        var result = _sut.Pop<object>("key", 3);

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.ShouldBeEquivalentTo(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: popping \"3\" items from \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void PopCount_WhenServiceReturnsError_ShouldReturnError()
    {
        var error = Error.New("some message");

        _mockService
            .Pop<object>("key", 3)
            .Returns(error);

        var result = _sut.Pop<object>("key", 3);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: popping \"3\" items from \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task PopCountAsync_WhenServiceReturnsData_ShouldReturnData()
    {
        var data = new[] { Option<object>.Some(new object()), Option<object>.None() };

        _mockService
            .PopAsync<object>("key", 3)
            .Returns(data);

        var result = await _sut.PopAsync<object>("key", 3);

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.ShouldBeEquivalentTo(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: async popping \"3\" items from \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task PopCountAsync_WhenServiceReturnsError_ShouldReturnError()
    {
        var error = Error.New("some message");

        _mockService
            .PopAsync<object>("key", 3)
            .Returns(error);

        var result = await _sut.PopAsync<object>("key", 3);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: async popping \"3\" items from \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}
