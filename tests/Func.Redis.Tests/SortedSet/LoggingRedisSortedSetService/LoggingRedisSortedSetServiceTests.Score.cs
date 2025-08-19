namespace Func.Redis.Tests.SortedSet.LoggingRedisSortedSetService;
internal partial class LoggingRedisSortedSetServiceTests
{
    [Test]
    public void Score_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var value = new object();
        _mockService
            .Score("key", value)
            .Returns(10.0.ToOption());

        var result = _sut.Score("key", value);

        result.IsRight.Should().BeTrue();
        result.OnRight(o =>
        {
            o.IsSome.Should().BeTrue();
            o.OnSome(l => l.Should().Be(10.0));
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: getting score of item in sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task ScoreAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var value = new object();
        _mockService
            .ScoreAsync("key", value)
            .Returns(10.0.ToOption());
     
        var result = await _sut.ScoreAsync("key", value);
        
        result.IsRight.Should().BeTrue();
        result.OnRight(o =>
        {
            o.IsSome.Should().BeTrue();
            o.OnSome(l => l.Should().Be(10.0));
        });
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async getting score of item in sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public void Score_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var value = new object();
        var error = Error.New("some error");
        _mockService
            .Score("key", value)
            .Returns(error);

        var result = _sut.Score("key", value);
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: getting score of item in sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task ScoreAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var value = new object();
        var error = Error.New("some error");
        _mockService
            .ScoreAsync("key", value)
            .Returns(error);
     
        var result = await _sut.ScoreAsync("key", value);
        
        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService: async getting score of item in sorted set at \"key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisSortedSetService raised an error with some error");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

}
