using static Func.Redis.Tests.TestDataElements;

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

        result.IsRight.Should().BeTrue();
        result.OnRight(e =>
        {
            e.IsSome.Should().BeTrue();
            e.OnSome(d => d.Should().Be(data));
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService: getting value at \"some key\" for index \"0\"");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService: getting value at \"some key\" for index \"0\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void Get_WhenServiceReturnsNone_ShouldReturnNone()
    {
        _mockService
            .Get<object>("some key", 0)
            .Returns(Option<object>.None());

        var result = _sut.Get<object>("some key", 0);

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.IsNone.Should().BeTrue());

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService: getting value at \"some key\" for index \"0\"");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsRight.Should().BeTrue();
        result.OnRight(d => d.Should().BeEquivalentTo(data));
    }

    [Test]
    public void GetInterval_WhenDatabaseReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");

        _mockService
            .Get<TestData>("some key", 0, 1)
            .Returns(error);

        var result = _sut.Get<TestData>("some key", 0, 1);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService: getting values at \"some key\" between \"0\" and \"1\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
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

        result.IsRight.Should().BeTrue();
        result.OnRight(e =>
        {
            e.IsSome.Should().BeTrue();
            e.OnSome(d => d.Should().Be(data));
        });

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService: async getting value at \"some key\" for index \"0\"");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService: async getting value at \"some key\" for index \"0\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task GetAsync_WhenServiceReturnsNone_ShouldReturnNone()
    {
        _mockService
            .GetAsync<object>("some key", 0)
            .Returns(Option<object>.None());

        var result = await _sut.GetAsync<object>("some key", 0);

        result.IsRight.Should().BeTrue();
        result.OnRight(e => e.IsNone.Should().BeTrue());

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService: async getting value at \"some key\" for index \"0\"");
            e.LogLevel.Should().Be(LogLevel.Information);
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

        result.IsRight.Should().BeTrue();
        result.OnRight(d => d.Should().BeEquivalentTo(data));
    }

    [Test]
    public async Task GetIntervalAsync_WhenDatabaseReturnsLeft_ShouldReturnLeftAndLog()
    {
        var error = Error.New("some message");

        _mockService
            .GetAsync<TestData>("some key", 0, 1)
            .Returns(error);

        var result = await _sut.GetAsync<TestData>("some key", 0, 1);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService: async getting values at \"some key\" between \"0\" and \"1\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}
