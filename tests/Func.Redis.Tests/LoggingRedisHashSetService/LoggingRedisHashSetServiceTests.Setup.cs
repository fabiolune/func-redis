namespace Func.Redis.Tests.LoggingRedisHashSetService;

public partial class LoggingRedisHashSetServiceTest
{
    private Redis.LoggingRedisHashSetService _sut;
    private ITestLoggerFactory _loggerFactory;
    private ILogger _mockLogger;
    private IRedisHashSetService _mockService;

    [SetUp]
    public void Setup()
    {
        _loggerFactory = MELTBuilder.CreateLoggerFactory();
        _mockLogger = _loggerFactory.CreateLogger<IRedisHashSetService>();
        _mockService = Substitute.For<IRedisHashSetService>();
        _sut = new Redis.LoggingRedisHashSetService(_mockLogger, _mockService);
    }

    private record TestData
    {
        public string Id { get; init; }
    }
}