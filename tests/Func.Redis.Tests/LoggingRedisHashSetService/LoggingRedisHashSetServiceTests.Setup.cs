﻿using Func.Redis.HashSet;

namespace Func.Redis.Tests.LoggingRedisHashSetService;

public partial class LoggingRedisHashSetServiceTest
{
    private HashSet.LoggingRedisHashSetService _sut;
    private ITestLoggerFactory _loggerFactory;
    private ILogger _mockLogger;
    private IRedisHashSetService _mockService;

    [SetUp]
    public void Setup()
    {
        _loggerFactory = TestLoggerFactory.Create();
        _mockLogger = _loggerFactory.CreateLogger<IRedisHashSetService>();
        _mockService = Substitute.For<IRedisHashSetService>();
        _sut = new HashSet.LoggingRedisHashSetService(_mockLogger, _mockService);
    }

    private record TestData
    {
        public string Id { get; init; }
    }
}