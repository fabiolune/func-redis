namespace Func.Redis.Tests.LoggingRedisKeyService;

public partial class LoggingRedisKeyServiceTests
{
    [Test]
    public async Task DeleteAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        _mockService
            .DeleteAsync("some key")
            .Returns(Unit.Default);

        var result = await _sut.DeleteAsync("some key");

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: async deleting key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task MultipleDeleteAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var keys = new[] { "key1", "key2" };
        _mockService
            .DeleteAsync(keys)
            .Returns(Unit.Default);

        var result = await _sut.DeleteAsync(keys);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: async deleting keys \"key1, key2\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task DeleteAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .DeleteAsync("some key")
            .Returns(error);

        var result = await _sut.DeleteAsync("some key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: async deleting key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultipleDeleteAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var keys = new[] { "key1", "key2" };
        var error = Error.New("some message");
        _mockService
            .DeleteAsync(keys)
            .Returns(error);

        var result = await _sut.DeleteAsync(keys);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: async deleting keys \"key1, key2\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task DeleteAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .DeleteAsync("some key")
            .Returns(error);

        var result = await _sut.DeleteAsync("some key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: async deleting key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultipleDeleteAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var keys = new[] { "key1", "key2" };
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .DeleteAsync(keys)
            .Returns(error);

        var result = await _sut.DeleteAsync(keys);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: async deleting keys \"key1, key2\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}