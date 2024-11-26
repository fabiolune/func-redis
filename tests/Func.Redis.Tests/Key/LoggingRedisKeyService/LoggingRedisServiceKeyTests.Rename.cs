namespace Func.Redis.Tests.LoggingRedisKeyService;

public partial class LoggingRedisKeyServiceTests
{
    [Test]
    public void RenameKey_WhenServiceReturnsSome_ShouldReturnSome()
    {
        _mockService
            .RenameKey("key", "key2")
            .Returns(Unit.Default);

        var result = _sut.RenameKey("key", "key2");

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void RenameKey_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .RenameKey("key", "key2")
            .Returns(error);

        var result = _sut.RenameKey("key", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void RenameKey_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);

        _mockService
            .RenameKey("key", "key2")
            .Returns(error);

        var result = _sut.RenameKey("key", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}