namespace Func.Redis.Tests.LoggingRedisService;

public partial class LoggingRedisServiceTests
{
    [Test]
    public async Task RenameKeyAsync_WhenServiceReturnsSome_ShouldReturnSome()
    {
        _mockService
            .RenameKeyAsync("key", "key2")
            .Returns(Unit.Default);

        var result = await _sut.RenameKeyAsync("key", "key2");

        result.IsRight.Should().BeTrue();

        _loggerFactory.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task RenameKeyAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .RenameKeyAsync("key", "key2")
            .Returns(error);

        var result = await _sut.RenameKeyAsync("key", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task RenameKeyAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);

        _mockService
            .RenameKeyAsync("key", "key2")
            .Returns(error);

        var result = await _sut.RenameKeyAsync("key", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}