namespace Func.Redis.Tests.LoggingRedisSetService;
internal partial class LoggingRedisSetServiceTests
{
    [Test]
    public void Size_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = 42;
        _mockService
            .Size("some key")
            .Returns(data);

        var result = _sut.Size("some key");

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().Be(data));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void Size_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .Size("some key")
            .Returns(error);

        var result = _sut.Size("some key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task SizeAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = 42;
        _mockService
            .SizeAsync("some key")
            .Returns(data);

        var result = await _sut.SizeAsync("some key");

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().Be(data));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task SizeAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .SizeAsync("some key")
            .Returns(error);

        var result = await _sut.SizeAsync("some key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}
