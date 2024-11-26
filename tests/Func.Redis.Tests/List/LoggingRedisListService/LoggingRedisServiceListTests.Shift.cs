namespace Func.Redis.Tests.LoggingRedisListService;
internal partial class LoggingRedisServiceListTests
{
    [Test]
    public void Shift_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object();

        _mockService
            .Shift<object>("some key")
            .Returns(data.ToOption());

        var result = _sut.Shift<object>("some key");

        result.IsRight.Should().BeTrue();
        result.OnRight(e =>
        {
            e.IsSome.Should().BeTrue();
            e.OnSome(d => d.Should().Be(data));
        });

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void Shift_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");

        _mockService
            .Shift<object>("some key")
            .Returns(error);

        var result = _sut.Shift<object>("some key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task ShiftAsync_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object();

        _mockService
            .ShiftAsync<object>("some key")
            .Returns(data.ToOption());

        var result = await _sut.ShiftAsync<object>("some key");

        result.IsRight.Should().BeTrue();
        result.OnRight(e =>
        {
            e.IsSome.Should().BeTrue();
            e.OnSome(d => d.Should().Be(data));
        });

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task ShiftAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");

        _mockService
            .ShiftAsync<object>("some key")
            .Returns(error);

        var result = await _sut.ShiftAsync<object>("some key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void ShiftCount_WhenServiceReturnsData_ShouldReturnData()
    {
        var data = new[] { Option<object>.Some(new object()), Option<object>.None() };

        _mockService
            .Shift<object>("key", 3)
            .Returns(data);

        var result = _sut.Shift<object>("key", 3);

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().BeEquivalentTo(data));
    }

    [Test]
    public void ShiftCount_WhenServiceReturnsError_ShouldReturnError()
    {
        var error = Error.New("some message");

        _mockService
            .Shift<object>("key", 3)
            .Returns(error);

        var result = _sut.Shift<object>("key", 3);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
    }

    [Test]
    public async Task ShiftCountAsync_WhenServiceReturnsData_ShouldReturnData()
    {
        var data = new[] { Option<object>.Some(new object()), Option<object>.None() };

        _mockService
            .ShiftAsync<object>("key", 3)
            .Returns(data);

        var result = await _sut.ShiftAsync<object>("key", 3);

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.Should().BeEquivalentTo(data));
    }

    [Test]
    public async Task ShiftCountAsync_WhenServiceReturnsError_ShouldReturnError()
    {
        var error = Error.New("some message");

        _mockService
            .ShiftAsync<object>("key", 3)
            .Returns(error);

        var result = await _sut.ShiftAsync<object>("key", 3);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));
    }
}
