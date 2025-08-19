using Func.Redis.SortedSet;

namespace Func.Redis.Tests.SortedSet.LoggingRedisSortedSetService;
internal partial class LoggingRedisSortedSetServiceTests
{
    private Redis.SortedSet.LoggingRedisSortedSetService _sut;
    private ITestLoggerFactory _loggerFactory;
    private IRedisSortedSetService _mockService;
    private ILogger<IRedisSortedSetService> _mockLogger;

    [SetUp]
    public void SetUp()
    {
        _loggerFactory = TestLoggerFactory.Create();

        _mockLogger = _loggerFactory.CreateLogger<IRedisSortedSetService>();
        _mockService = Substitute.For<IRedisSortedSetService>();
        _sut = new Redis.SortedSet.LoggingRedisSortedSetService(_mockLogger, _mockService);
    }
}
