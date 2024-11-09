using Func.Redis.Logging;

namespace Func.Redis.Tests;

public class LoggingRedisPublisherServiceTests
{
    private LoggingRedisPublisherService _sut;
    private ITestLoggerFactory _loggerFactory;
    private ILogger _mockLogger;
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
       
        _loggerFactory
            .Sink
            .LogEntries
            .Should()
            .BeEmpty();
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

        _loggerFactory
            .Sink
            .LogEntries
            .Should()
            .ContainSingle(e => e.LogLevel == LogLevel.Error && e.Message == "IRedisPublisherService raised an error with some message");
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

        _loggerFactory
            .Sink
            .LogEntries
            .Should()
            .ContainSingle(e => e.LogLevel == LogLevel.Error && e.Message == "IRedisPublisherService raised an error with some message");
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

        _loggerFactory
            .Sink
            .LogEntries
            .Should()
            .BeEmpty();
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

        _loggerFactory
            .Sink
            .LogEntries
            .Should()
            .ContainSingle(e => e.LogLevel == LogLevel.Error && e.Message == "IRedisPublisherService raised an error with some message");
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

        _loggerFactory
            .Sink
            .LogEntries
            .Should()
            .ContainSingle(e => e.LogLevel == LogLevel.Error && e.Message == "IRedisPublisherService raised an error with some message");
    }

    #endregion

}