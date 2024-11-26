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

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().BeEquivalentTo(data));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void GetAll_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .GetAll<object>("key")
            .Returns(error);

        var result = _sut.GetAll<object>("key");

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
    public async Task GetAllAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data1 = new object();
        var data2 = new object();
        var data = new[] { data1.ToOption(), data2.ToOption(), Option<object>.None() };
        _mockService
            .GetAllAsync<object>("key")
            .Returns(data);

        var result = await _sut.GetAllAsync<object>("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().BeEquivalentTo(data));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task GetAllAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");
        _mockService
            .GetAllAsync<object>("key")
            .Returns(error);

        var result = await _sut.GetAllAsync<object>("key");

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