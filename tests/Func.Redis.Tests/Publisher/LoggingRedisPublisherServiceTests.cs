using Func.Redis.Publisher;

namespace Func.Redis.Tests.Publisher;

public class LoggingRedisPublisherServiceTests
{
    private LoggingRedisPublisherService _sut;
    private ITestLoggerFactory _loggerFactory;
    private ILogger<IRedisPublisherService> _mockLogger;
    private IRedisPublisherService _mockService;

    [SetUp]
    public void Setup()
    {
        _loggerFactory = TestLoggerFactory.Create();
        _mockLogger = _loggerFactory.CreateLogger<IRedisPublisherService>();
        _mockService = Substitute.For<IRedisPublisherService>();
        _sut = new LoggingRedisPublisherService(_mockService, _mockLogger);
    }

    #region PublishMessage

    [Test]
    public void PublishMessage_WhenServiceReturnsRight_ShouldReturnRight()
    {
        var data = new object();
        _mockService
            .Publish("some channel", data)
            .Returns(Unit.Default);

        var result = _sut.Publish("some channel", data);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisPublisherService: publishing message to \"some channel\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public void PublishMessage_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        var data = new object();
        _mockService
            .Publish("some channel", data)
            .Returns(error);

        var result = _sut.Publish("some channel", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisPublisherService: publishing message to \"some channel\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisPublisherService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void PublishMessage_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        var data = new object();
        _mockService
            .Publish("some channel", data)
            .Returns(error);

        var result = _sut.Publish("some channel", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisPublisherService: publishing message to \"some channel\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisPublisherService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    #endregion

    #region PublishMessageAsync

    [Test]
    public async Task PublishMessageAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data = new object();
        _mockService
            .PublishAsync("some channel", data)
            .Returns(Unit.Default);

        var result = await _sut.PublishAsync("some channel", data);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisPublisherService: async publishing message to \"some channel\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task PublishMessageAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        var data = new object();
        _mockService
            .PublishAsync("some channel", data)
            .Returns(error);

        var result = await _sut.PublishAsync("some channel", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisPublisherService: async publishing message to \"some channel\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisPublisherService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task PublishMessageAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        var data = new object();
        _mockService
            .PublishAsync("some channel", data)
            .Returns(error);

        var result = await _sut.PublishAsync("some channel", data);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisPublisherService: async publishing message to \"some channel\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisPublisherService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    #endregion

}