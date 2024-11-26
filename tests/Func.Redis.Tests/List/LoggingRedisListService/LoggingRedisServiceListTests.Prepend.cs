
namespace Func.Redis.Tests.LoggingRedisListService;
internal partial class LoggingRedisServiceListTests
{
    [Test]
    public void Prepend_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object();

        _mockService
            .Prepend("some key", data)
            .Returns(Unit.Default);

        var result = _sut.Prepend("some key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void Prepend_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        var data = new object();

        _mockService
            .Prepend("some key", data)
            .Returns(error);

        var result = _sut.Prepend("some key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void MultiplePrepend_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var value1 = new object();
        var value2 = new object();

        var data = new[] { value1, value2 };

        _mockService
            .Prepend("key", data)
            .Returns(Unit.Default);

        var result = _sut.Prepend("key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void MultiplePrepend_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        var value1 = new object();
        var value2 = new object();

        var data = new[] { value1, value2 };

        _mockService
            .Prepend("key", data)
            .Returns(error);

        var result = _sut.Prepend("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task PrependAsync_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object();

        _mockService
            .PrependAsync("some key", data)
            .Returns(Unit.Default);

        var result = await _sut.PrependAsync("some key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task PrependAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        var data = new object();

        _mockService
            .PrependAsync("some key", data)
            .Returns(error);

        var result = await _sut.PrependAsync("some key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultiplePrependAsync_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var value1 = new object();
        var value2 = new object();

        var data = new[] { value1, value2 };

        _mockService
            .PrependAsync("key", data)
            .Returns(Unit.Default);

        var result = await _sut.PrependAsync("key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task MultiplePrependAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        var value1 = new object();
        var value2 = new object();

        var data = new[] { value1, value2 };

        _mockService
            .PrependAsync("key", data)
            .Returns(error);

        var result = await _sut.PrependAsync("key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}