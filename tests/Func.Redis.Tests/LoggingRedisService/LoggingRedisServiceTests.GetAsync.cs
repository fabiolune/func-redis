namespace Func.Redis.Tests.LoggingRedisService;

public partial class LoggingRedisServiceTests
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

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.OnSome(d => d.Should().Be(data)));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task MultipleGetAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data1 = new object();
        var data2 = new object();
        var output = new[] { Option<object>.Some(data1), Option<object>.Some(data2) };
        _mockService
            .GetAsync<object>("key1", "key2")
            .Returns(output);

        var result = await _sut.GetAsync<object>("key1", "key2");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.Filter().Should().BeEquivalentTo(new[] { data1, data2 }));

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task GetAsync_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        _mockService
            .GetAsync<object>("some key")
            .Returns(Either<Error, Option<object>>.Right(Option<object>.None()));

        var result = await _sut.GetAsync<object>("some key");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.IsNone.Should().BeTrue());

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: key \"some key\" not found");
            e.LogLevel.Should().Be(LogLevel.Warning);
        });
    }

    [Test]
    public async Task MultipleGetAsync_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        var output = new[] { Option<object>.None(), Option<object>.None() };
        _mockService
            .GetAsync<object>("key1", "key2")
            .Returns(output);

        var result = await _sut.GetAsync<object>("key1", "key2");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.Filter().Should().BeEmpty());

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public async Task GetAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .GetAsync<object>("some key")
            .Returns(error);

        var result = await _sut.GetAsync<object>("some key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultipleGetAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .GetAsync<object>("key1", "key2")
            .Returns(error);

        var result = await _sut.GetAsync<object>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultipleGetAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .GetAsync<object>("key1", "key2")
            .Returns(error);

        var result = await _sut.GetAsync<object>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}