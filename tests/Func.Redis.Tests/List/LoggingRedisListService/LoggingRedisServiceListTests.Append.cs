namespace Func.Redis.Tests.LoggingRedisListService;
internal partial class LoggingRedisServiceListTests
{
    [Test]
    public void Append_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object();

        _mockService
            .Append("some key", data)
            .Returns(Unit.Default);

        var result = _sut.Append("some key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void Append_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        var data = new object();

        _mockService
            .Append("some key", data)
            .Returns(error);

        var result = _sut.Append("some key", data);

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
    public void MultipleAppend_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var value1 = new object();
        var value2 = new object();

        var data = new[] { value1, value2 };

        _mockService
            .Append("key", data)
            .Returns(Unit.Default);

        var result = _sut.Append("key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void MultipleAppend_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        var value1 = new object();
        var value2 = new object();

        var data = new[] { value1, value2 };

        _mockService
            .Append("key", data)
            .Returns(error);

        var result = _sut.Append("key", data);

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
    public async Task AppendAsync_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object();

        _mockService
            .AppendAsync("some key", data)
            .Returns(Unit.Default);

        var result = await _sut.AppendAsync("some key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task AppendAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        var data = new object();

        _mockService
            .AppendAsync("some key", data)
            .Returns(error);

        var result = await _sut.AppendAsync("some key", data);

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
    public async Task MultipleAppendAsync_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var value1 = new object();
        var value2 = new object();

        var data = new[] { value1, value2 };

        _mockService
            .AppendAsync("key", data)
            .Returns(Unit.Default);

        var result = await _sut.AppendAsync("key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task MultipleAppendAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        var value1 = new object();
        var value2 = new object();

        var data = new[] { value1, value2 };

        _mockService
            .AppendAsync("key", data)
            .Returns(error);

        var result = await _sut.AppendAsync("key", data);

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