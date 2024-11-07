namespace Func.Redis.Tests.LoggingRedisService;

public partial class LoggingRedisServiceTests
{
    [Test]
    public void Delete_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        _mockService
            .Delete("some key")
            .Returns(Either<Error, Unit>.Right(Unit.Default));

        var result = _sut.Delete("some key");

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void MultipleDelete_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        _mockService
            .Delete("key1", "key2")
            .Returns(Unit.Default);

        var result = _sut.Delete("key1", "key2");

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
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

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void MultipleDelete_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .Delete("key1", "key2")
            .Returns(error);

        var result = _sut.Delete("key1", "key2");

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

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void Delete_WhenServiceReturnsLeftWithException_ShouldReturnLeftMultiple()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .Delete("key1", "key2")
            .Returns(Either<Error, Unit>.Left(error));

        var result = _sut.Delete("key1", "key2");

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