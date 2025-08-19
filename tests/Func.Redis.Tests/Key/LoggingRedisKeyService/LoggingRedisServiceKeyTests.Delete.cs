namespace Func.Redis.Tests.LoggingRedisKeyService;

public partial class LoggingRedisKeyServiceTests
{
    [Test]
    public void Delete_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        _mockService
            .Delete("some key")
            .Returns(Either<Error, Unit>.Right(Unit.Default));

        var result = _sut.Delete("some key");

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: deleting key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public void MultipleDelete_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var keys = new[] { "key1", "key2" };
        _mockService
            .Delete(keys)
            .Returns(Unit.Default);

        var result = _sut.Delete(keys);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: deleting keys \"key1, key2\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public void Delete_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .Delete("some key")
            .Returns(error);

        var result = _sut.Delete("some key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: deleting key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void MultipleDelete_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var keys = new[] { "key1", "key2" };
        var error = Error.New("some message");
        _mockService
            .Delete(keys)
            .Returns(error);

        var result = _sut.Delete(keys);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: deleting keys \"key1, key2\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void Delete_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .Delete("some key")
            .Returns(error);

        var result = _sut.Delete("some key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: deleting key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void Delete_WhenServiceReturnsLeftWithException_ShouldReturnLeftMultiple()
    {
        var keys = new[] { "key1", "key2" };
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .Delete(keys)
            .Returns(error);

        var result = _sut.Delete(keys);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: deleting keys \"key1, key2\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}