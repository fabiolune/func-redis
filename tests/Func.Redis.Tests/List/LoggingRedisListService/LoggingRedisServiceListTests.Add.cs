namespace Func.Redis.Tests.LoggingRedisListService;
internal partial class LoggingRedisServiceListTests
{
    [Test]
    public void Get_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object();

        _mockService
            .Get<object>("some key", 0)
            .Returns(data.ToOption());

        var result = _sut.Get<object>("some key", 0);

        result.IsRight.ShouldBeTrue();
        result.OnRight(e =>
        {
            e.IsSome.ShouldBeTrue();
            e.OnSome(d => d.ShouldBe(data));
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: getting value at \"some key\" for index \"0\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void Get_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");

        _mockService
            .Get<object>("some key", 0)
            .Returns(error);

        var result = _sut.Get<object>("some key", 0);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: getting value at \"some key\" for index \"0\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public void Get_WhenServiceReturnsNone_ShouldReturnNone()
    {
        _mockService
            .Get<object>("some key", 0)
            .Returns(Option<object>.None());

        var result = _sut.Get<object>("some key", 0);

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.IsNone.ShouldBeTrue());

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: getting value at \"some key\" for index \"0\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void GetInterval_WhenDatabaseReturnsRight_ShouldReturnRight()
    {
        var value = new TestData(1);
        var data = new[] { value.ToOption(), Option<TestData>.None() };

        _mockService
            .Get<TestData>("some key", 0, 1)
            .Returns(data);

        var result = _sut.Get<TestData>("some key", 0, 1);

        result.IsRight.ShouldBeTrue();
        result.OnRight(d => d.ShouldBeEquivalentTo(data));
    }

    [Test]
    public void GetInterval_WhenDatabaseReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");

        _mockService
            .Get<TestData>("some key", 0, 1)
            .Returns(error);

        var result = _sut.Get<TestData>("some key", 0, 1);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: getting values at \"some key\" between \"0\" and \"1\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task GetAsync_WhenServiceReturnsSome_ShouldReturnSome()
    {
        var data = new object();

        _mockService
            .GetAsync<object>("some key", 0)
            .Returns(data.ToOption());

        var result = await _sut.GetAsync<object>("some key", 0);

        result.IsRight.ShouldBeTrue();
        result.OnRight(e =>
        {
            e.IsSome.ShouldBeTrue();
            e.OnSome(d => d.ShouldBe(data));
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: async getting value at \"some key\" for index \"0\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task GetAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");

        _mockService
            .GetAsync<object>("some key", 0)
            .Returns(error);

        var result = await _sut.GetAsync<object>("some key", 0);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: async getting value at \"some key\" for index \"0\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task GetAsync_WhenServiceReturnsNone_ShouldReturnNone()
    {
        _mockService
            .GetAsync<object>("some key", 0)
            .Returns(Option<object>.None());

        var result = await _sut.GetAsync<object>("some key", 0);

        result.IsRight.ShouldBeTrue();
        result.OnRight(e => e.IsNone.ShouldBeTrue());

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: async getting value at \"some key\" for index \"0\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task GetIntervalAsync_WhenDatabaseReturnsRight_ShouldReturnRight()
    {
        var value = new TestData(1);
        var data = new[] { value.ToOption(), Option<TestData>.None() };

        _mockService
            .GetAsync<TestData>("some key", 0, 1)
            .Returns(data);

        var result = await _sut.GetAsync<TestData>("some key", 0, 1);

        result.IsRight.ShouldBeTrue();
        result.OnRight(d => d.ShouldBeEquivalentTo(data));
    }

    [Test]
    public async Task GetIntervalAsync_WhenDatabaseReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");

        _mockService
            .GetAsync<TestData>("some key", 0, 1)
            .Returns(error);

        var result = await _sut.GetAsync<TestData>("some key", 0, 1);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService: async getting values at \"some key\" between \"0\" and \"1\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisListService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}
