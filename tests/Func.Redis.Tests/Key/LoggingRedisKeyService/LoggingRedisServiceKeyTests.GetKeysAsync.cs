namespace Func.Redis.Tests.LoggingRedisKeyService;

public partial class LoggingRedisKeyServiceTests
{
    [Test]
    public async Task GetKeysAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .GetKeysAsync("some pattern")
            .Returns(error);

        var result = await _sut.GetKeysAsync("some pattern");

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
    public async Task GetKeysAsync_WhenServiceReturnsErrorWithException_ShouldReturnLeftAndLog()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .GetKeysAsync("some pattern")
            .Returns(error);

        var result = await _sut.GetKeysAsync("some pattern");

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
    public async Task GetKeysAsync_WhenServiceReturnsData_ShouldReturnRight()
    {
        var data = new[]
        {
            "key1",
            "key2"
        };
        _mockService
            .GetKeysAsync("some pattern")
            .Returns(data);

        var result = await _sut.GetKeysAsync("some pattern");

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().BeEquivalentTo(data));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }
}