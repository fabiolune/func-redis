namespace Func.Redis.Tests.LoggingRedisSetService;
internal partial class LoggingRedisSetServiceTests
{
    [Test]
    public void Delete_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new object();
        _mockService
            .Delete("some key", data)
            .Returns(Unit.Default);

        var result = _sut.Delete("some key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void Delete_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var data = new object();
        var error = Error.New("some message");
        _mockService
            .Delete("some key", data)
            .Returns(error);

        var result = _sut.Delete("some key", data);

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
    public async Task DeleteAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new object();
        _mockService
            .DeleteAsync("some key", data)
            .Returns(Unit.Default);

        var result = await _sut.DeleteAsync("some key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task DeleteAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var data = new object();
        var error = Error.New("some message");
        _mockService
            .DeleteAsync("some key", data)
            .Returns(error);

        var result = await _sut.DeleteAsync("some key", data);

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
    public void DeleteMultiple_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new object[] { new(), new() };
        _mockService
            .Delete("some key", data)
            .Returns(Unit.Default);

        var result = _sut.Delete("some key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void DeleteMultiple_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var data = new object[] { new(), new() };
        var error = Error.New("some message");
        _mockService
            .Delete("some key", data)
            .Returns(error);

        var result = _sut.Delete("some key", data);

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
    public async Task DeleteMultipleAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new object[] { new(), new() };
        _mockService
            .DeleteAsync("some key", data)
            .Returns(Unit.Default);

        var result = await _sut.DeleteAsync("some key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task DeleteMultipleAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var data = new object[] { new(), new() };
        var error = Error.New("some message");
        _mockService
            .DeleteAsync("some key", data)
            .Returns(error);

        var result = await _sut.DeleteAsync("some key", data);

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
