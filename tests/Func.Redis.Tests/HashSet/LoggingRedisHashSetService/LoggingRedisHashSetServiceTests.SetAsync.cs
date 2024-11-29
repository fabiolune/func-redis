namespace Func.Redis.Tests.LoggingRedisHashSetService;

public partial class LoggingRedisHashSetServiceTest
{

    [Test]
    public async Task SetAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data = new object();
        _mockService
            .SetAsync("some key", "some field", data)
            .Returns(Unit.Default);

        var result = await _sut.SetAsync("some key", "some field", data);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: async setting field \"some field\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task SetAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        var data = new object();
        _mockService
            .SetAsync("some key", "some field", data)
            .Returns(error);

        var result = await _sut.SetAsync("some key", "some field", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: async setting field \"some field\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task SetAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        var data = new object();
        _mockService
            .SetAsync("some key", "some field", data)
            .Returns(error);

        var result = await _sut.SetAsync("some key", "some field", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: async setting field \"some field\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
    [Test]
    public async Task MultiSetAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data = new object();
        var pairs = new (string, object)[] { ("some field 1", data), ("some field 2", data) };
        _mockService
            .SetAsync("some key", pairs)
            .Returns(Unit.Default);

        var result = await _sut.SetAsync("some key", pairs);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: async setting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task MultiSetAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var data = new object();
        var pairs = new (string, object)[] { ("some field 1", data), ("some field 2", data) };
        var error = Error.New("some message");
        _mockService
            .SetAsync("some key", pairs)
            .Returns(error);

        var result = await _sut.SetAsync("some key", pairs);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: async setting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultiSetAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var data = new object();
        var pairs = new (string, object)[] { ("some field 1", data), ("some field 2", data) };
        var exception = new Exception("some message");
        var error = Error.New(exception);

        _mockService
            .SetAsync("some key", pairs)
            .Returns(error);

        var result = await _sut.SetAsync("some key", pairs);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: async setting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}