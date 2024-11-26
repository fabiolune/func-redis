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

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.OnSome(d => d.Should().BeEquivalentTo(data)));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();

    }

    [Test]
    public void GetFieldKeys_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        var output = Option<string[]>.None();
        _mockService
            .GetFieldKeys("some key")
            .Returns(output);

        var result = _sut.GetFieldKeys("some key");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.IsNone.Should().BeTrue());

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: the key \"some key\" contains no fields");
            e.LogLevel.Should().Be(LogLevel.Warning);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}