namespace Func.Redis.Tests.LoggingRedisSetService;
internal partial class LoggingRedisSetServiceTests
{
    [Test]
    public void Pop_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object().ToOption();
        _mockService
            .Pop<object>("key")
            .Returns(data);

        var result = _sut.Pop<object>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().Be(data));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void Pop_WhenServiceReturnsNone_ShouldReturnNone()
    {
        var data = Option<object>.None();
        _mockService
            .Pop<object>("key")
            .Returns(data);

        var result = _sut.Pop<object>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().Be(data));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void Pop_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .Pop<object>("key")
            .Returns(error);

        var result = _sut.Pop<object>("key");

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
    public async Task PopAsync_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object().ToOption();
        _mockService
            .PopAsync<object>("key")
            .Returns(data);

        var result = await _sut.PopAsync<object>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().Be(data));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task PopAsync_WhenServiceReturnsNone_ShouldReturnNone()
    {
        var data = Option<object>.None();
        _mockService
            .PopAsync<object>("key")
            .Returns(data);

        var result = await _sut.PopAsync<object>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().Be(data));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task PopAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .PopAsync<object>("key")
            .Returns(error);

        var result = await _sut.PopAsync<object>("key");

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
