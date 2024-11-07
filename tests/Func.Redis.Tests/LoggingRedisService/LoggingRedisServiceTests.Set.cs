namespace Func.Redis.Tests.LoggingRedisService;

public partial class LoggingRedisServiceTests
{
    [Test]
    public void Set_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data = new object();
        _mockService
            .Set("some key", data)
            .Returns(Unit.Default);

        var result = _sut.Set("some key", data);

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void MultipleSet_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data1 = new object();
        var data2 = new object();
        _mockService
            .Set(Arg.Is<(string, object)>(t => t.Item1 == "key1" && t.Item2 == data1), Arg.Is<(string, object)>(t => t.Item1 == "key2" && t.Item2 == data2))
            .Returns(Unit.Default);

        var result = _sut.Set(("key1", data1), ("key2", data2));

        result.IsRight.Should().BeTrue();

        _loggerFactory.Sink.LogEntries.Should().BeEmpty();
    }

    [Test]
    public void Set_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        var data = new object();
        _mockService
            .Set("some key", data)
            .Returns(error);

        var result = _sut.Set("some key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void MultipleSet_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var data1 = new object();
        var data2 = new object();
        var error = Error.New("some message");
        _mockService
            .Set(Arg.Is<(string, object)>(t => t.Item1 == "key1" && t.Item2 == data1), Arg.Is<(string, object)>(t => t.Item1 == "key2" && t.Item2 == data2))
            .Returns(error);

        var result = _sut.Set(("key1", data1), ("key2", data2));

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void Set_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        var data = new object();
        _mockService
            .Set("some key", data)
            .Returns(error);

        var result = _sut.Set("some key", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void MultipleSet_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        var data1 = new object();
        var data2 = new object();
        _mockService
            .Set(Arg.Is<(string, object)>(t => t.Item1 == "key1" && t.Item2 == data1), Arg.Is<(string, object)>(t => t.Item1 == "key2" && t.Item2 == data2))
            .Returns(error);

        var result = _sut.Set(("key1", data1), ("key2", data2));

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries;
        entries.Should().HaveCount(1);
        entries.First().Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}