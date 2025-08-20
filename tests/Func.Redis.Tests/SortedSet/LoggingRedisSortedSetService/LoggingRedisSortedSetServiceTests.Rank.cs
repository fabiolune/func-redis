namespace Func.Redis.Tests.SortedSet.LoggingRedisSortedSetService;
internal partial class LoggingRedisSortedSetServiceTests
{
    [Test]
    public void Rank_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var value = new object();
        _mockService
            .Rank("key", value)
            .Returns(10L.ToOption());

        var result = _sut.Rank("key", value);

        result.IsRight.ShouldBeTrue();
        result.OnRight(o =>
        {
            o.IsSome.ShouldBeTrue();
            o.OnSome(l => l.ShouldBe(10L));
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: getting rank of item in sorted set at \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task RankAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var value = new object();
        _mockService
            .RankAsync("key", value)
            .Returns(10L.ToOption());

        var result = await _sut.RankAsync("key", value);

        result.IsRight.ShouldBeTrue();
        result.OnRight(o =>
        {
            o.IsSome.ShouldBeTrue();
            o.OnSome(l => l.ShouldBe(10L));
        });
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async getting rank of item in sorted set at \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void Rank_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var value = new object();
        var error = Error.New("some error");
        _mockService
            .Rank("key", value)
            .Returns(error);

        var result = _sut.Rank("key", value);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: getting rank of item in sorted set at \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task RankAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var value = new object();
        var error = Error.New("some error");
        _mockService
            .RankAsync("key", value)
            .Returns(error);

        var result = await _sut.RankAsync("key", value);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));
        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async getting rank of item in sorted set at \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

}
