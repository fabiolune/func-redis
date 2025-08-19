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

        result.IsRight.Should().BeTrue();
        result.OnRight(values => values.Should().BeEquivalentTo(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: union of keys [key1, key2]");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsRight.Should().BeTrue();
        result.OnRight(values => values.Should().BeEquivalentTo(data));
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async union of keys [key1, key2]");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: union of keys [key1, key2]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async union of keys [key1, key2]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}
