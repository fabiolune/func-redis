namespace Func.Redis.Tests.SortedSet.LoggingRedisSortedSetService;
internal partial class LoggingRedisSortedSetServiceTests
{
    [Test]
    public void Union_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var keys = new[] { "key1", "key2" };
        var data = new[] { new object(), new object() };
        _mockService
            .Union<object>(keys)
            .Returns(data);

        var result = _sut.Union<object>(keys);

        result.IsRight.ShouldBeTrue();
        result.OnRight(values => values.ShouldBeEquivalentTo(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: union of keys [key1, key2]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task UnionAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var keys = new[] { "key1", "key2" };
        var data = new[] { new object(), new object() };
        _mockService
            .UnionAsync<object>(keys)
            .Returns(data);

        var result = await _sut.UnionAsync<object>(keys);

        result.IsRight.ShouldBeTrue();
        result.OnRight(values => values.ShouldBeEquivalentTo(data));
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async union of keys [key1, key2]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void Union_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var keys = new[] { "key1", "key2" };
        var error = Error.New("Some error");
        _mockService
            .Union<object>(keys)
            .Returns(error);
        var result = _sut.Union<object>(keys);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: union of keys [key1, key2]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task UnionAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var keys = new[] { "key1", "key2" };
        var error = Error.New("Some error");
        _mockService
            .UnionAsync<object>(keys)
            .Returns(error);

        var result = await _sut.UnionAsync<object>(keys);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async union of keys [key1, key2]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}
