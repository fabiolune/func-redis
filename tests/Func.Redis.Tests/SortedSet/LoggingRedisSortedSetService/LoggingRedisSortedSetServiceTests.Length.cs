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

        result.IsRight.Should().BeTrue();
        result.OnRight(l => l.Should().Be(10));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: getting length of sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: getting length of sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task LengthAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .LengthAsync("key")
            .Returns(10);

        var result = await _sut.LengthAsync("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(l => l.Should().Be(10));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async getting length of sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async getting length of sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void LengthByScore_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .LengthByScore("key", 1, 10)
            .Returns(5);
        
        var result = _sut.LengthByScore("key", 1, 10);
        
        result.IsRight.Should().BeTrue();
        result.OnRight(l => l.Should().Be(5));
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: getting length of sorted set at \"key\" with score range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
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
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
        
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: getting length of sorted set at \"key\" with score range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task LengthByScoreAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .LengthByScoreAsync("key", 1, 10)
            .Returns(5);
        
        var result = await _sut.LengthByScoreAsync("key", 1, 10);
        
        result.IsRight.Should().BeTrue();
        result.OnRight(l => l.Should().Be(5));
        
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async getting length of sorted set at \"key\" with score range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
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
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
        
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async getting length of sorted set at \"key\" with score range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void LengthByValue_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .LengthByValue("key", 1, 10)
            .Returns(5);
        
        var result = _sut.LengthByValue("key", 1, 10);
        
        result.IsRight.Should().BeTrue();
        result.OnRight(l => l.Should().Be(5));
     
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: getting length of sorted set at \"key\" with value range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
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
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
        
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: getting length of sorted set at \"key\" with value range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task LengthByValueAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        _mockService
            .LengthByValueAsync("key", 1, 10)
            .Returns(5);
        
        var result = await _sut.LengthByValueAsync("key", 1, 10);
        
        result.IsRight.Should().BeTrue();
        result.OnRight(l => l.Should().Be(5));
        
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async getting length of sorted set at \"key\" with value range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
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
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
        
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async getting length of sorted set at \"key\" with value range [1, 10]");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}
