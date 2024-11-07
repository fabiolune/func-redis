namespace Func.Redis.Tests.LoggingRedisService;

public partial class LoggingRedisServiceTests
{
    private LoggingRedisKeyService _sut;
    private ITestLoggerFactory _loggerFactory;
    private ILogger _mockLogger;
    private IRedisKeyService _mockService;

    [SetUp]
    public void Setup()
    {
        _loggerFactory = TestLoggerFactory.Create();

        _mockLogger = _loggerFactory.CreateLogger<IRedisKeyService>();
        _mockService = Substitute.For<IRedisKeyService>();
        _sut = new LoggingRedisKeyService(_mockLogger, _mockService);
    }
}