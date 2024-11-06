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

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.OnSome(d => d.Should().Be(data)));

        _loggerFactory.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task GetAsync_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        var output = Option<object>.None();
        _mockService
            .GetAsync<object>("some key", "some field")
            .Returns(output);

        var result = await _sut.GetAsync<object>("some key", "some field");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.IsNone.Should().BeTrue());

        var entries = _loggerFactory.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: the key \"some key\" does not contain the field \"some field\"");
            e.LogLevel.Should().Be(LogLevel.Warning);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsRight.Should().BeTrue();
        result.OnRight(r =>
        {
            r.Should().HaveCount(2);
            r.Filter().Should().BeEquivalentTo([data, data]);
        });

        _loggerFactory.LogEntries.Should().BeEmpty();
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

        result.IsRight.Should().BeTrue();
        result.OnRight(r =>
        {
            r.Should().HaveCount(2);
            r.Filter().Should().BeEmpty();
        });

        _loggerFactory.LogEntries.Should().BeEmpty();
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsRight.Should().BeTrue();
        result.OnRight(r =>
        {
            r.Should().HaveCount(2);
            r.Filter().Should().BeEquivalentTo([data, data]);
        });

        _loggerFactory.LogEntries.Should().BeEmpty();
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

        result.IsRight.Should().BeTrue();
        result.OnRight(r =>
        {
            r.Should().HaveCount(2);
            r.Filter().Should().BeEmpty();
        });

        _loggerFactory.LogEntries.Should().BeEmpty();
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}