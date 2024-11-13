﻿using Func.Redis.Extensions;
using Microsoft.Extensions.Logging;

namespace Func.Redis.Publisher;

public class LoggingRedisPublisherService(IRedisPublisherService redisPublisherService, ILogger logger) : IRedisPublisherService
{
    private readonly IRedisPublisherService _redisPublisherService = redisPublisherService;
    private readonly ILogger _logger = logger;

    private const string ComponentName = nameof(IRedisPublisherService);

    public Either<Error, Unit> Publish(string channel, object message) =>
        _redisPublisherService
            .Publish(channel, message)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> PublishAsync(string channel, object message) =>
        _redisPublisherService
            .PublishAsync(channel, message)
            .TeeLog(_logger, ComponentName);
}