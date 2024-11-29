using static Func.Redis.Tests.TestDataElements;

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

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.OnSome(d => d.Should().Be(data)));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: getting field \"some field\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.IsNone.Should().BeTrue());

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: getting field \"some field\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: the key \"some key\" does not contain the field \"some field\"");
            e.LogLevel.Should().Be(LogLevel.Warning);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: getting field \"some field\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: getting field \"some field\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsRight.Should().BeTrue();
        result.OnRight(r =>
        {
            r.Should().HaveCount(2);
            r.Filter().Should().BeEquivalentTo([data, data]);
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsRight.Should().BeTrue();
        result.OnRight(r =>
        {
            r.Should().HaveCount(2);
            r.Filter().Should().BeEmpty();
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsRight.Should().BeTrue();
        result.OnRight(r =>
        {
            r.Should().HaveCount(2);
            r.Filter().Should().BeEquivalentTo([data, data]);
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsRight.Should().BeTrue();
        result.OnRight(r =>
        {
            r.Should().HaveCount(2);
            r.Filter().Should().BeEmpty();
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: getting fields \"some field 1, some field 2\" for key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}