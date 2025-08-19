namespace Func.Redis.Tests.SortedSet.LoggingRedisSortedSetService;
internal partial class LoggingRedisSortedSetServiceTests
{
    [Test]
    public void Remove_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var value = new object();
        _mockService
            .Remove("key", value)
            .Returns(Unit.Default);

        var result = _sut.Remove("key", value);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: removing item from sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task RemoveAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var value = new object();
        _mockService
            .RemoveAsync("key", value)
            .Returns(Unit.Default);

        var result = await _sut.RemoveAsync("key", value);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async removing item from sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public void Remove_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var value = new object();
        var error = Error.New("some error");
        _mockService
            .Remove("key", value)
            .Returns(error);

        var result = _sut.Remove("key", value);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: removing item from sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task RemoveAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var value = new object();
        var error = Error.New("some error");
        _mockService
            .RemoveAsync("key", value)
            .Returns(error);

        var result = await _sut.RemoveAsync("key", value);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async removing item from sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void Remove_WhenServiceReturnsRightForMultipleValues_ShouldReturnRight()
    {
        var values = new[] { new object(), new object() };
        _mockService
            .Remove<object>("key", values)
            .Returns(Unit.Default);

        var result = _sut.Remove<object>("key", values);

        result.IsRight.Should().BeTrue();
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: removing items from sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task RemoveAsync_WhenServiceReturnsRightForMultipleValues_ShouldReturnRight()
    {
        var values = new[] { new object(), new object() };
        _mockService
            .RemoveAsync<object>("key", values)
            .Returns(Unit.Default);

        var result = await _sut.RemoveAsync<object>("key", values);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async removing items from sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public void Remove_WhenServiceReturnsLeftForMultipleValues_ShouldReturnLeftAndLog()
    {
        var values = new[] { new object(), new object() };
        var error = Error.New("some error");
        _mockService
            .Remove<object>("key", values)
            .Returns(error);

        var result = _sut.Remove<object>("key", values);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: removing items from sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task RemoveAsync_WhenServiceReturnsLeftForMultipleValues_ShouldReturnLeftAndLog()
    {
        var values = new[] { new object(), new object() };
        var error = Error.New("some error");
        _mockService
            .RemoveAsync<object>("key", values)
            .Returns(error);

        var result = await _sut.RemoveAsync<object>("key", values);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async removing items from sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void RemoveRangeByScore_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .RemoveRangeByScore("key", 1.0, 10.0)
            .Returns(Unit.Default);

        var result = _sut.RemoveRangeByScore("key", 1.0, 10.0);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: removing range by score from sorted set at \"key\" with range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task RemoveRangeByScoreAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .RemoveRangeByScoreAsync("key", 1.0, 10.0)
            .Returns(Unit.Default);

        var result = await _sut.RemoveRangeByScoreAsync("key", 1.0, 10.0);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async removing range by score from sorted set at \"key\" with range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public void RemoveRangeByScore_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some error");
        _mockService
            .RemoveRangeByScore("key", 1.0, 10.0)
            .Returns(error);

        var result = _sut.RemoveRangeByScore("key", 1.0, 10.0);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: removing range by score from sorted set at \"key\" with range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task RemoveRangeByScoreAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some error");
        _mockService
            .RemoveRangeByScoreAsync("key", 1.0, 10.0)
            .Returns(error);

        var result = await _sut.RemoveRangeByScoreAsync("key", 1.0, 10.0);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async removing range by score from sorted set at \"key\" with range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void RemoveRangeByValue_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var min = new object();
        var max = new object();
        _mockService
            .RemoveRangeByValue("key", min, max)
            .Returns(Unit.Default);
        var result = _sut.RemoveRangeByValue("key", min, max);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: removing range by value from sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task RemoveRangeByValueAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var min = new object();
        var max = new object();
        _mockService
            .RemoveRangeByValueAsync("key", min, max)
            .Returns(Unit.Default);

        var result = await _sut.RemoveRangeByValueAsync("key", min, max);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async removing range by value from sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public void RemoveRangeByValue_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var min = new object();
        var max = new object();
        var error = Error.New("some error");
        _mockService
            .RemoveRangeByValue("key", min, max)
            .Returns(error);

        var result = _sut.RemoveRangeByValue("key", min, max);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: removing range by value from sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task RemoveRangeByValueAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var min = new object();
        var max = new object();
        var error = Error.New("some error");
        _mockService
            .RemoveRangeByValueAsync("key", min, max)
            .Returns(error);

        var result = await _sut.RemoveRangeByValueAsync("key", min, max);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async removing range by value from sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}
