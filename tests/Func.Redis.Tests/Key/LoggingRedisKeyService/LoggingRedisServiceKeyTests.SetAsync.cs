namespace Func.Redis.Tests.LoggingRedisKeyService;

public partial class LoggingRedisKeyServiceTests
{
    [Test]
    public async Task SetAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data = new object();
        _mockService
            .SetAsync("some key", data)
            .Returns(Unit.Default);

        var result = await _sut.SetAsync("some key", data);

        result.IsRight.ShouldBeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async setting key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task MultipleSetAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data1 = new object();
        var data2 = new object();
        _mockService
            .SetAsync(Arg.Is<(string, object)>(t => t.Item1 == "key1" && t.Item2 == data1), Arg.Is<(string, object)>(t => t.Item1 == "key2" && t.Item2 == data2))
            .Returns(Unit.Default);

        var result = await _sut.SetAsync(("key1", data1), ("key2", data2));

        result.IsRight.ShouldBeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async setting keys \"key1, key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task SetAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        var data = new object();
        _mockService
            .SetAsync("some key", data)
            .Returns(error);

        var result = await _sut.SetAsync("some key", data);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async setting key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultipleSetAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var data1 = new object();
        var data2 = new object();
        var error = Error.New("some message");
        _mockService
            .SetAsync(Arg.Is<(string, object)>(t => t.Item1 == "key1" && t.Item2 == data1), Arg.Is<(string, object)>(t => t.Item1 == "key2" && t.Item2 == data2))
            .Returns(error);

        var result = await _sut.SetAsync(("key1", data1), ("key2", data2));

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async setting keys \"key1, key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task SetAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        var data = new object();
        _mockService
            .SetAsync("some key", data)
            .Returns(error);

        var result = await _sut.SetAsync("some key", data);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async setting key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultipleSetAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        var data1 = new object();
        var data2 = new object();
        _mockService
            .SetAsync(Arg.Is<(string, object)>(t => t.Item1 == "key1" && t.Item2 == data1), Arg.Is<(string, object)>(t => t.Item1 == "key2" && t.Item2 == data2))
            .Returns(error);

        var result = await _sut.SetAsync(("key1", data1), ("key2", data2));

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async setting keys \"key1, key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}