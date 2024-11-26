namespace Func.Redis.Tests.LoggingRedisSetService;
internal partial class LoggingRedisSetServiceTests
{
    [Test]
    public void Add_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new object();
        _mockService
            .Add("some key", data)
            .Returns(Unit.Default);

        var result = _sut.Add("some key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void Add_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var data = new object();
        var error = Error.New("some message");
        _mockService
            .Add("some key", data)
            .Returns(error);

        var result = _sut.Add("some key", data);

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
    public async Task AddAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new object();
        _mockService
            .AddAsync("some key", data)
            .Returns(Unit.Default);

        var result = await _sut.AddAsync("some key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task AddAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var data = new object();
        var error = Error.New("some message");
        _mockService
            .AddAsync("some key", data)
            .Returns(error);

        var result = await _sut.AddAsync("some key", data);

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
    public void Add_WhenServiceRetunsRightWithNone_ShouldReturnRightWithNone()
    {
        _mockService
            .Add("some key", Arg.Any<object>())
            .Returns(Unit.Default);

        var result = _sut.Add("some key", new object());

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task AddAsync_WhenServiceRetunsRightWithNone_ShouldReturnRightWithNone()
    {
        _mockService
            .AddAsync("some key", Arg.Any<object>())
            .Returns(Unit.Default);

        var result = await _sut.AddAsync("some key", new object());

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void AddMultiple_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new object[] { new(), new() };
        _mockService
            .Add("some key", data)
            .Returns(Unit.Default);

        var result = _sut.Add("some key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void AddMultiple_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var data = new object[] { new(), new() };
        var error = Error.New("some message");
        _mockService
            .Add("some key", data)
            .Returns(error);

        var result = _sut.Add("some key", data);

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
    public async Task AddMultipleAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new object[] { new(), new() };
        _mockService
            .AddAsync("some key", data)
            .Returns(Unit.Default);

        var result = await _sut.AddAsync("some key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task AddMultipleAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var data = new object[] { new(), new() };
        var error = Error.New("some message");
        _mockService
            .AddAsync("some key", data)
            .Returns(error);

        var result = await _sut.AddAsync("some key", data);

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
