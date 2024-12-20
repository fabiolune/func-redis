﻿using Func.Redis.Set;

namespace Func.Redis.Tests.LoggingRedisSetService;
internal partial class LoggingRedisSetServiceTests
{
    private Redis.Set.LoggingRedisSetService _sut;
    private ITestLoggerFactory _loggerFactory;
    private ILogger<IRedisSetService> _mockLogger;
    private IRedisSetService _mockService;

    [SetUp]
    public void Setup()
    {
        _loggerFactory = TestLoggerFactory.Create();

        _mockLogger = _loggerFactory.CreateLogger<IRedisSetService>();
        _mockService = Substitute.For<IRedisSetService>();
        _sut = new Redis.Set.LoggingRedisSetService(_mockLogger, _mockService);
    }
}
