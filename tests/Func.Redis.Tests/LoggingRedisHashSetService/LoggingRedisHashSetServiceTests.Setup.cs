namespace Func.Redis.Tests.LoggingRedisHashSetService;

public partial class LoggingRedisHashSetServiceTest
{
    private Logging.LoggingRedisHashSetService _sut;
    private ITestLoggerFactory _loggerFactory;
    private ILogger _mockLogger;
    private IRedisHashSetService _mockService;

    [SetUp]
    public void Setup()
    {
        _loggerFactory = TestLoggerFactory.Create();
        _mockLogger = _loggerFactory.CreateLogger<IRedisHashSetService>();
        _mockService = Substitute.For<IRedisHashSetService>();
        _sut = new Redis.LoggingRedisHashSetService(_mockLogger, _mockService);
    }

    private record TestData
    {
        public string Id { get; init; }
    }
}