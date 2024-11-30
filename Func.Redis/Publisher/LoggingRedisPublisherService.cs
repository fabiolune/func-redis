using Func.Redis.Extensions;
using Microsoft.Extensions.Logging;
using TinyFp.Extensions;

namespace Func.Redis.Publisher;

internal class LoggingRedisPublisherService(
    IRedisPublisherService redisPublisherService,
    ILogger<IRedisPublisherService> logger) : IRedisPublisherService
{
    private readonly IRedisPublisherService _redisPublisherService = redisPublisherService;
    private readonly ILogger _logger = logger;

    private const string ComponentName = nameof(IRedisPublisherService);

    public Either<Error, Unit> Publish(string channel, object message) =>
        (channel, message)
            .Tee(t => _logger.LogInformation("{Component}: publishing message to \"{Channel}\"", ComponentName, t.channel))
            .Map(t => _redisPublisherService.Publish(t.channel, t.message))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> PublishAsync(string channel, object message) =>
        (channel, message)
            .Tee(t => _logger.LogInformation("{Component}: async publishing message to \"{Channel}\"", ComponentName, t.channel))
            .Map(t => _redisPublisherService.PublishAsync(t.channel, t.message))
            .TeeLog(_logger, ComponentName);
}