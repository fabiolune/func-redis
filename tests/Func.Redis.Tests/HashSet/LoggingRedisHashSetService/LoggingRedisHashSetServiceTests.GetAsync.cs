namespace Func.Redis.Tests.LoggingRedisHashSetService;

public partial class LoggingRedisHashSetServiceTest
{
    [Test]
    public async Task GetAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data = new object();
        var output = Option<object>.Some(data);
        _mockService
            .GetAsync<object>("some key", "some field")
            .Returns(output);

        var result = await _sut.GetAsync<object>("some key", "some field");

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.OnSome(d => d.ShouldBe(data)));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: async getting field \"some field\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task GetAsync_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        var output = Option<object>.None();
        _mockService
            .GetAsync<object>("some key", "some field")
            .Returns(output);

        var result = await _sut.GetAsync<object>("some key", "some field");

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.IsNone.ShouldBeTrue());

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: async getting field \"some field\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: the key \"some key\" does not contain the field \"some field\"");
            e.LogLevel.ShouldBe(LogLevel.Warning);
        });
    }

    [Test]
    public async Task GetAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .GetAsync<object>("some key", "some field")
            .Returns(error);

        var result = await _sut.GetAsync<object>("some key", "some field");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: async getting field \"some field\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task GetAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .GetAsync<object>("some key", "some field")
            .Returns(error);

        var result = await _sut.GetAsync<object>("some key", "some field");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: async getting field \"some field\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultiGetAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var fields = new[] { "some field 1", "some field 2" };
        var data = new object();
        var output = new[] { Option<object>.Some(data), Option<object>.Some(data) };
        _mockService
            .GetAsync<object>("some key", fields)
            .Returns(output);

        var result = await _sut.GetAsync<object>("some key", fields);

        result.IsRight.ShouldBeTrue();
        result.OnRight(r =>
        {
            r.Length.ShouldBe(2);
            var filtered = r.Filter().ToArray();
            filtered.Length.ShouldBe(2);
            filtered[0].ShouldBe(data);
            filtered[1].ShouldBe(data);
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: async getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task MultiGetAsync_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        var fields = new[] { "some field 1", "some field 2" };
        var output = new[] { Option<object>.None(), Option<object>.None() };
        _mockService
            .GetAsync<object>("some key", fields)
            .Returns(output);

        var result = await _sut.GetAsync<object>("some key", fields);

        result.IsRight.ShouldBeTrue();
        result.OnRight(r =>
        {
            r.Length.ShouldBe(2);
            r.Filter().ShouldBeEmpty();
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: async getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task MultiGetAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var fields = new[] { "some field 1", "some field 2" };
        var error = Error.New("some message");
        _mockService
            .GetAsync<object>("some key", fields)
            .Returns(error);

        var result = await _sut.GetAsync<object>("some key", fields);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: async getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultiGetAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var fields = new[] { "some field 1", "some field 2" };
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .GetAsync<object>("some key", fields)
            .Returns(error);

        var result = await _sut.GetAsync<object>("some key", fields);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: async getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultiGetTypeAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var fields = new[] { (typeof(TestData), "some field 1"), (typeof(TestData), "some field 2") };
        var data = new object();
        var output = new[] { Option<object>.Some(data), Option<object>.Some(data) };
        _mockService
            .GetAsync("some key", fields)
            .Returns(output);

        var result = await _sut.GetAsync("some key", fields);

        result.IsRight.ShouldBeTrue();
        result.OnRight(r =>
        {
            r.Length.ShouldBe(2);
            var filtered = r.Filter().ToArray();
            filtered.Length.ShouldBe(2);
            filtered[0].ShouldBe(data);
            filtered[1].ShouldBe(data);
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: async getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task MultiGetTypeAsync_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        var fields = new[] { (typeof(TestData), "some field 1"), (typeof(TestData), "some field 2") };
        var output = new[] { Option<object>.None(), Option<object>.None() };
        _mockService
            .GetAsync("some key", fields)
            .Returns(output);

        var result = await _sut.GetAsync("some key", fields);

        result.IsRight.ShouldBeTrue();
        result.OnRight(r =>
        {
            r.Length.ShouldBe(2);
            r.Filter().ShouldBeEmpty();
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: async getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task MultiGetTypeAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var fields = new[] { (typeof(TestData), "some field 1"), (typeof(TestData), "some field 2") };
        var error = Error.New("some message");
        _mockService
            .GetAsync("some key", fields)
            .Returns(error);

        var result = await _sut.GetAsync("some key", fields);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: async getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultiGetTypeAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var fields = new[] { (typeof(TestData), "some field 1"), (typeof(TestData), "some field 2") };
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .GetAsync("some key", fields)
            .Returns(error);

        var result = await _sut.GetAsync("some key", fields);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: async getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}