using Func.Redis.List;

namespace Func.Redis.Tests.LoggingRedisListService;
internal partial class LoggingRedisServiceListTests
{
    private Redis.List.LoggingRedisListService _sut;
    private ITestLoggerFactory _loggerFactory;
    private ILogger<IRedisListService> _mockLogger;
    private IRedisListService _mockService;

    [SetUp]
    public void Setup()
    {
        _loggerFactory = TestLoggerFactory.Create();

        _mockLogger = _loggerFactory.CreateLogger<IRedisListService>();
        _mockService = Substitute.For<IRedisListService>();
        _sut = new Redis.List.LoggingRedisListService(_mockLogger, _mockService);
    }
}
