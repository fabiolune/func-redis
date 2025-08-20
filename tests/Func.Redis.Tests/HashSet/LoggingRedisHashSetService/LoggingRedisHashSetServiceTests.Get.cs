namespace Func.Redis.Tests.LoggingRedisHashSetService;

public partial class LoggingRedisHashSetServiceTest
{
    [Test]
    public void Get_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data = new object();
        var output = Option<object>.Some(data);
        _mockService
            .Get<object>("some key", "some field")
            .Returns(output);

        var result = _sut.Get<object>("some key", "some field");

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.OnSome(d => d.ShouldBe(data)));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: getting field \"some field\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void Get_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        var output = Option<object>.None();
        _mockService
            .Get<object>("some key", "some field")
            .Returns(output);

        var result = _sut.Get<object>("some key", "some field");

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.IsNone.ShouldBeTrue());

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: getting field \"some field\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: the key \"some key\" does not contain the field \"some field\"");
            e.LogLevel.ShouldBe(LogLevel.Warning);
        });
    }

    [Test]
    public void Get_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .Get<object>("some key", "some field")
            .Returns(error);

        var result = _sut.Get<object>("some key", "some field");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: getting field \"some field\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public void Get_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .Get<object>("some key", "some field")
            .Returns(error);

        var result = _sut.Get<object>("some key", "some field");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: getting field \"some field\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public void MultiGet_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var fields = new[] { "some field 1", "some field 2" };
        var data = new object();
        var output = new[] { Option<object>.Some(data), Option<object>.Some(data) };
        _mockService
            .Get<object>("some key", fields)
            .Returns(output);

        var result = _sut.Get<object>("some key", fields);

        result.IsRight.ShouldBeTrue();
        result.OnRight(r =>
        {
            r.Length.ShouldBe(2);
            var filtered = r.Filter().ToArray();
            filtered.Length.ShouldBe(2);
            filtered[0].ShouldBe(data);
            filtered[1].ShouldBe(data);
            //r.Filter().ShouldBeEquivalentTo([data, data]);
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(1);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void MultiGet_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        var fields = new[] { "some field 1", "some field 2" };
        var output = new[] { Option<object>.None(), Option<object>.None() };
        _mockService
            .Get<object>("some key", fields)
            .Returns(output);

        var result = _sut.Get<object>("some key", fields);

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
            e.Message.ShouldBe("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void MultiGet_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var fields = new[] { "some field 1", "some field 2" };
        var error = Error.New("some message");
        _mockService
            .Get<object>("some key", fields)
            .Returns(error);

        var result = _sut.Get<object>("some key", fields);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public void MultiGet_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var fields = new[] { "some field 1", "some field 2" };
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .Get<object>("some key", fields)
            .Returns(error);

        var result = _sut.Get<object>("some key", fields);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public void MultiGetType_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var fields = new[] { (typeof(TestData), "some field 1"), (typeof(TestData), "some field 2") };
        var data = new object();
        var output = new[] { Option<object>.Some(data), Option<object>.Some(data) };
        _mockService
            .Get("some key", fields)
            .Returns(output);

        var result = _sut.Get("some key", fields);

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
            e.Message.ShouldBe("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void MultiGetType_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        var fields = new[] { (typeof(TestData), "some field 1"), (typeof(TestData), "some field 2") };
        var output = new[] { Option<object>.None(), Option<object>.None() };
        _mockService
            .Get("some key", fields)
            .Returns(output);

        var result = _sut.Get("some key", fields);

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
            e.Message.ShouldBe("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
    }

    [Test]
    public void MultiGetType_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var fields = new[] { (typeof(TestData), "some field 1"), (typeof(TestData), "some field 2") };
        var error = Error.New("some message");
        _mockService
            .Get("some key", fields)
            .Returns(error);

        var result = _sut.Get("some key", fields);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }

    [Test]
    public void MultiGetType_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var fields = new[] { (typeof(TestData), "some field 1"), (typeof(TestData), "some field 2") };
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .Get("some key", fields)
            .Returns(error);

        var result = _sut.Get("some key", fields);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Length.ShouldBe(2);
        entries[0].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.ShouldBe(LogLevel.Information);
        });
        entries[1].ShouldBeOfType<LogEntry>().Tee(e =>
        {
            e.Message.ShouldBe("IRedisHashSetService raised an error with some message");
            e.LogLevel.ShouldBe(LogLevel.Error);
        });
    }
}