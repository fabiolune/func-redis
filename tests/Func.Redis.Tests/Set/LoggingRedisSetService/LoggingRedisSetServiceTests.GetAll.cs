namespace Func.Redis.Tests.LoggingRedisSetService;
internal partial class LoggingRedisSetServiceTests
{
    [Test]
    public void GetAll_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data1 = new object();
        var data2 = new object();
        var data = new[] { data1.ToOption(), data2.ToOption(), Option<object>.None() };
        _mockService
            .GetAll<object>("key")
            .Returns(data);

        var result = _sut.GetAll<object>("key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.ShouldBeEquivalentTo(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: getting all items for \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void GetAll_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .GetAll<object>("key")
            .Returns(error);

        var result = _sut.GetAll<object>("key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: getting all items for \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task GetAllAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data1 = new object();
        var data2 = new object();
        var data = new[] { data1.ToOption(), data2.ToOption(), Option<object>.None() };
        _mockService
            .GetAllAsync<object>("key")
            .Returns(data);

        var result = await _sut.GetAllAsync<object>("key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.ShouldBeEquivalentTo(data));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: async getting all items for \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task GetAllAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .GetAllAsync<object>("key")
            .Returns(error);

        var result = await _sut.GetAllAsync<object>("key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService: async getting all items for \"key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}