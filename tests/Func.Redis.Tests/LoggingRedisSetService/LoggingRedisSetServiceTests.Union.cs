namespace Func.Redis.Tests.LoggingRedisSetService;
internal partial class LoggingRedisSetServiceTests
{
    [Test]
    public void Union_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new[] { new object(), new object() };
        _mockService
            .Union<object>("key1", "key2")
            .Returns(data);

        var result = _sut.Union<object>("key1", "key2");

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().BeEquivalentTo(data));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void Union_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .Union<object>("key1", "key2")
            .Returns(error);

        var result = _sut.Union<object>("key1", "key2");

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
    public async Task UnionAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new[] { new object(), new object() };
        _mockService
            .UnionAsync<object>("key1", "key2")
            .Returns(data);

        var result = await _sut.UnionAsync<object>("key1", "key2");

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().BeEquivalentTo(data));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task UnionAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .UnionAsync<object>("key1", "key2")
            .Returns(error);

        var result = await _sut.UnionAsync<object>("key1", "key2");

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
