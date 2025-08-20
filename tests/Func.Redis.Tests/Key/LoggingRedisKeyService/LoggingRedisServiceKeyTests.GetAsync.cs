namespace Func.Redis.Tests.LoggingRedisKeyService;

public partial class LoggingRedisKeyServiceTests
{
    [Test]
    public async Task GetAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data = new object();
        var output = Option<object>.Some(data);
        _mockService
            .GetAsync<object>("some key")
            .Returns(Either<Error, Option<object>>.Right(output));

        var result = await _sut.GetAsync<object>("some key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.OnSome(d => d.ShouldBe(data)));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async getting key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task MultipleGetAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var keys = new[] { "key1", "key2" };
        var data1 = new object();
        var data2 = new object();
        var output = new[] { Option<object>.Some(data1), Option<object>.Some(data2) };
        _mockService
            .GetAsync<object>(keys)
            .Returns(output);

        var result = await _sut.GetAsync<object>(keys);

        result.IsRight.ShouldBeTrue();
        result.OnRight(r =>
        {
            r.Length.ShouldBe(2);
            r.Filter().ShouldBe([data1, data2]);
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async getting keys \"key1, key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task GetAsync_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        _mockService
            .GetAsync<object>("some key")
            .Returns(Either<Error, Option<object>>.Right(Option<object>.None()));

        var result = await _sut.GetAsync<object>("some key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.IsNone.ShouldBeTrue());

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async getting key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: key \"some key\" not found");
            e.LogLevel.ShouldBe(LogLevel.Warning);
        });
    }

    [Test]
    public async Task MultipleGetAsync_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        var keys = new[] { "key1", "key2" };
        var output = new[] { Option<object>.None(), Option<object>.None() };
        _mockService
            .GetAsync<object>(keys)
            .Returns(output);

        var result = await _sut.GetAsync<object>(keys);

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.Filter().ShouldBeEmpty());

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async getting keys \"key1, key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public async Task GetAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .GetAsync<object>("some key")
            .Returns(error);

        var result = await _sut.GetAsync<object>("some key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(r => r.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async getting key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultipleGetAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var keys = new[] { "key1", "key2" };
        var error = Error.New("some message");
        _mockService
            .GetAsync<object>(keys)
            .Returns(error);

        var result = await _sut.GetAsync<object>(keys);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(r => r.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async getting keys \"key1, key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task GetAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .GetAsync<object>("some key")
            .Returns(error);

        var result = await _sut.GetAsync<object>("some key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(r => r.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async getting key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultipleGetAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var keys = new[] { "key1", "key2" };
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .GetAsync<object>(keys)
            .Returns(error);

        var result = await _sut.GetAsync<object>(keys);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(r => r.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService: async getting keys \"key1, key2\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisKeyService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}