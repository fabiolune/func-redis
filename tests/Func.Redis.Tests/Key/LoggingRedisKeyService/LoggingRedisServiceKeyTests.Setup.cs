using Func.Redis.Key;

namespace Func.Redis.Tests.LoggingRedisKeyService;

public partial class LoggingRedisKeyServiceTests
{
    private Redis.Key.LoggingRedisKeyService _sut;
    private ITestLoggerFactory _loggerFactory;
    private ILogger<IRedisKeyService> _mockLogger;
    private IRedisKeyService _mockService;

    [SetUp]
    public void Setup()
    {
        _loggerFactory = TestLoggerFactory.Create();

        _mockLogger = _loggerFactory.CreateLogger<IRedisKeyService>();
        _mockService = Substitute.For<IRedisKeyService>();
        _sut = new Redis.Key.LoggingRedisKeyService(_mockLogger, _mockService);
    }
}