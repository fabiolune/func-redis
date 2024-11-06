namespace Func.Redis.Tests.LoggingRedisService;

public partial class LoggingRedisServiceTests
{
    [Test]
    public void GetKeys_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .GetKeys("some pattern")
            .Returns(error);

        var result = _sut.GetKeys("some pattern");

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
    public void GetKeys_WhenServiceReturnsErrorWithException_ShouldReturnLeftAndLog()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .GetKeys("some pattern")
            .Returns(error);

        var result = _sut.GetKeys("some pattern");

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

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().BeEquivalentTo(data));

        _loggerFactory.LogEntries.Should().BeEmpty();
    }
}