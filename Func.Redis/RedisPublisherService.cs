using Func.Redis.SerDes;
using StackExchange.Redis;
using TinyFp;

using static TinyFp.Prelude;

namespace Func.Redis;

public class RedisPublisherService(ISourcesProvider dbProvider, IRedisSerDes serDes) : IRedisPublisherService
{
    private readonly IDatabase _database = dbProvider.GetDatabase();
    private readonly IRedisSerDes _serDes = serDes;

    public Either<Error, Unit> Publish(string channel, object message) =>
        Try(() =>
            _database
                .Publish(RedisChannel.Literal(channel), _serDes.Serialize(message)))
        .ToEither()
        .MapLeft(e => Error.New(e))
        .Map(_ => Unit.Default);

    public Task<Either<Error, Unit>> PublishAsync(string channel, object message) =>
        TryAsync(() =>
            _database
                .PublishAsync(RedisChannel.Literal(channel), _serDes.Serialize(message)))
        .ToEither()
        .MapLeftAsync(e => Error.New(e))
        .MapAsync(_ => Unit.Default);
}