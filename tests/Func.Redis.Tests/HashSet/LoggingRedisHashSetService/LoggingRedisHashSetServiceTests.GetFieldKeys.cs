namespace Func.Redis.Tests.LoggingRedisHashSetService;

public partial class LoggingRedisHashSetServiceTest
{
    [Test]
    public void GetFieldKeys_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data = Array.Empty<string>();
        var output = Option<string[]>.Some(data);
        _mockService
            .GetFieldKeys("some key")
            .Returns(output);

        var result = _sut.GetFieldKeys("some key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.OnSome(d => d.ShouldBeEquivalentTo(data)));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: getting field keys for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void GetFieldKeys_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        var output = Option<string[]>.None();
        _mockService
            .GetFieldKeys("some key")
            .Returns(output);

        var result = _sut.GetFieldKeys("some key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.IsNone.ShouldBeTrue());

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: getting field keys for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: the key \"some key\" contains no fields");
            e.LogLevel.ShouldBe(LogLevel.Warning);
        });
    }

    [Test]
    public void GetFieldKeys_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .GetFieldKeys("some key")
            .Returns(error);

        var result = _sut.GetFieldKeys("some key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: getting field keys for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public void GetFieldKeys_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .GetFieldKeys("some key")
            .Returns(error);

        var result = _sut.GetFieldKeys("some key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: getting field keys for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}