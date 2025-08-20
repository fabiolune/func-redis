namespace Func.Redis.Tests.SortedSet.LoggingRedisSortedSetService;
internal partial class LoggingRedisSortedSetServiceTests
{
    [Test]
    public void Length_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .Length("key")
            .Returns(10);

        var result = _sut.Length("key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(l => l.ShouldBe(10));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: getting length of sorted set at \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void Length_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some error");
        _mockService
            .Length("key")
            .Returns(error);

        var result = _sut.Length("key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: getting length of sorted set at \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task LengthAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .LengthAsync("key")
            .Returns(10);

        var result = await _sut.LengthAsync("key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(l => l.ShouldBe(10));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async getting length of sorted set at \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task LengthAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some error");
        _mockService
            .LengthAsync("key")
            .Returns(error);

        var result = await _sut.LengthAsync("key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async getting length of sorted set at \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public void LengthByScore_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .LengthByScore("key", 1, 10)
            .Returns(5);

        var result = _sut.LengthByScore("key", 1, 10);

        result.IsRight.ShouldBeTrue();
        result.OnRight(l => l.ShouldBe(5));
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: getting length of sorted set at \"key\" with score range [1, 10]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void LengthByScore_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some error");
        _mockService
            .LengthByScore("key", 1, 10)
            .Returns(error);

        var result = _sut.LengthByScore("key", 1, 10);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: getting length of sorted set at \"key\" with score range [1, 10]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task LengthByScoreAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .LengthByScoreAsync("key", 1, 10)
            .Returns(5);

        var result = await _sut.LengthByScoreAsync("key", 1, 10);

        result.IsRight.ShouldBeTrue();
        result.OnRight(l => l.ShouldBe(5));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async getting length of sorted set at \"key\" with score range [1, 10]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task LengthByScoreAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some error");
        _mockService
            .LengthByScoreAsync("key", 1, 10)
            .Returns(error);

        var result = await _sut.LengthByScoreAsync("key", 1, 10);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async getting length of sorted set at \"key\" with score range [1, 10]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public void LengthByValue_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .LengthByValue("key", 1, 10)
            .Returns(5);

        var result = _sut.LengthByValue("key", 1, 10);

        result.IsRight.ShouldBeTrue();
        result.OnRight(l => l.ShouldBe(5));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: getting length of sorted set at \"key\" with value range [1, 10]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void LengthByValue_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some error");
        _mockService
            .LengthByValue("key", 1, 10)
            .Returns(error);

        var result = _sut.LengthByValue("key", 1, 10);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: getting length of sorted set at \"key\" with value range [1, 10]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task LengthByValueAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .LengthByValueAsync("key", 1, 10)
            .Returns(5);

        var result = await _sut.LengthByValueAsync("key", 1, 10);

        result.IsRight.ShouldBeTrue();
        result.OnRight(l => l.ShouldBe(5));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async getting length of sorted set at \"key\" with value range [1, 10]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task LengthByValueAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some error");
        _mockService
            .LengthByValueAsync("key", 1, 10)
            .Returns(error);

        var result = await _sut.LengthByValueAsync("key", 1, 10);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async getting length of sorted set at \"key\" with value range [1, 10]");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}
