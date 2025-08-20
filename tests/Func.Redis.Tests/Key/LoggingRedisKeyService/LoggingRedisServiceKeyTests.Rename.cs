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

        result.IsRight.ShouldBeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: renaming key \"key\" to \"key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void RenameKey_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .RenameKey("key", "key2")
            .Returns(error);

        var result = _sut.RenameKey("key", "key2");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: renaming key \"key\" to \"key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
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

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(r => r.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: renaming key \"key\" to \"key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}