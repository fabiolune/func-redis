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

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.ShouldBeEquivalentTo(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: getting union between \"key1\" and \"key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void Union_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .Union<object>("key1", "key2")
            .Returns(error);

        var result = _sut.Union<object>("key1", "key2");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: getting union between \"key1\" and \"key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
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

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.ShouldBeEquivalentTo(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: async getting union between \"key1\" and \"key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task UnionAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .UnionAsync<object>("key1", "key2")
            .Returns(error);

        var result = await _sut.UnionAsync<object>("key1", "key2");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: async getting union between \"key1\" and \"key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}
