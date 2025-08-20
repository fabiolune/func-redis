namespace Func.Redis.Tests.LoggingRedisKeyService;

public partial class LoggingRedisKeyServiceTests
{
    [Test]
    public void GetKeys_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .GetKeys("some pattern")
            .Returns(error);

        var result = _sut.GetKeys("some pattern");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: getting keys with pattern \"some pattern\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public void GetKeys_WhenServiceReturnsErrorWithException_ShouldReturnLeftAndLog()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .GetKeys("some pattern")
            .Returns(error);

        var result = _sut.GetKeys("some pattern");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: getting keys with pattern \"some pattern\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public void GetKeys_WhenServiceReturnsData_ShouldReturnRight()
    {
        var data = new[]
        {
            "key1",
            "key2"
        };
        _mockService
            .GetKeys("some pattern")
            .Returns(data);

        var result = _sut.GetKeys("some pattern");

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.ShouldBeEquivalentTo(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: getting keys with pattern \"some pattern\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }
}