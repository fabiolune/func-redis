using static Func.Redis.Tests.TestDataElements;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
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

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
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

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
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

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
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

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
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

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
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

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
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

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisListService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}
