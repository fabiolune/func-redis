namespace Func.Redis.Tests.SortedSet.LoggingRedisSortedSetService;
internal partial class LoggingRedisSortedSetServiceTests
{
    [Test]
    public void Add_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new object();
        _mockService
            .Add("some key", data, 10)
            .Returns(Unit.Default);

        var result = _sut.Add("some key", data, 10);

        result.IsRight.ShouldBeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: adding item to \"some key\" with score 10");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task AddAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new object();
        _mockService
            .AddAsync("some key", data, 10)
            .Returns(Either<Error, Unit>.Right(Unit.Default));

        var result = await _sut.AddAsync("some key", data, 10);

        result.IsRight.ShouldBeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async adding item to \"some key\" with score 10");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void Add_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("Some error");
        _mockService
            .Add("some key", "value", 10)
            .Returns(error);

        var result = _sut.Add("some key", "value", 10);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: adding item to \"some key\" with score 10");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task AddAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("Some error");
        _mockService
            .AddAsync("some key", "value", 10)
            .Returns(Either<Error, Unit>.Left(error));

        var result = await _sut.AddAsync("some key", "value", 10);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async adding item to \"some key\" with score 10");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public void AddMultiple_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new List<(object Value, double Score)>
        {
            ("value1", 1.0),
            ("value2", 2.0)
        };
        _mockService
            .Add("some key", data)
            .Returns(Unit.Default);

        var result = _sut.Add("some key", data);

        result.IsRight.ShouldBeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: adding items to \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task AddMultipleAsync_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new List<(object Value, double Score)>
        {
            ("value1", 1.0),
            ("value2", 2.0)
        };
        _mockService
            .AddAsync("some key", data)
            .Returns(Either<Error, Unit>.Right(Unit.Default));
        var result = await _sut.AddAsync("some key", data);

        result.IsRight.ShouldBeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async adding items to \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void AddMultiple_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var data = new List<(object Value, double Score)>
        {
            ("value1", 1.0),
            ("value2", 2.0)
        };
        var error = Error.New("Some error");
        _mockService
            .Add("some key", data)
            .Returns(error);
        var result = _sut.Add("some key", data);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: adding items to \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task AddMultipleAsync_WhenServiceReturnsLeft_ShouldReturnLeftAndLog()
    {
        var data = new List<(object Value, double Score)>
        {
            ("value1", 1.0),
            ("value2", 2.0)
        };
        var error = Error.New("Some error");
        _mockService
            .AddAsync("some key", data)
            .Returns(Either<Error, Unit>.Left(error));

        var result = await _sut.AddAsync("some key", data);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService: async adding items to \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisSortedSetService raised an error with Some error");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}
