namespace Func.Redis.Tests;

public class LoggingRedisPublisherServiceTests
{
    private LoggingRedisPublisherService _sut;
    private ILogger _mockLogger;
    private IRedisPublisherService _mockService;

    [SetUp]
    public void Setup()
    {
        _mockLogger = Substitute.For<ILogger>();
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
        _mockLogger
            .DidNotReceiveWithAnyArgs()
            .Log(default, default, default, default, default);
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
        //_mockLogger
        //    .Received(1)
        //    .Log(LogLevel.Error, 0, "IRedisPublisherService raised an error with some message", null, null);
        //.LogError("{Component} raised an error with {Message}", "IRedisPublisherService", "some message");
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
        //_mockLogger
        //    .Received(1)
        //    .LogError("{Component} raised an error with {Message}", "IRedisPublisherService", "some message");
        //_mockLogger
        //    .DidNotReceiveWithAnyArgs()
        //    .LogError(default);
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
        _mockLogger
            .DidNotReceiveWithAnyArgs()
            .LogError(default);
        _mockLogger
            .DidNotReceiveWithAnyArgs()
            .LogError(default(Exception), default);

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
        //_mockLogger
        //    .Received(1)
        //    .LogError("{Component} raised an error with {Message}", ["IRedisPublisherService", "some message"]);
        //_mockLogger
        //    .DidNotReceiveWithAnyArgs()
        //    .LogError(default(Exception), default);
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
        //_mockLogger
        //    .Received(1)
        //    .LogError(exception, "{Component} raised an error with {Message}", ["IRedisPublisherService", "some message"]);
        //_mockLogger
        //    .DidNotReceiveWithAnyArgs()
        //    .LogError(default(Exception), default);
    }

    #endregion

}