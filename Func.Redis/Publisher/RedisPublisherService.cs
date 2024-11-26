using Func.Redis.SerDes;
using static Func.Redis.Utils.FunctionUtilities;
using static Func.Redis.Utils.FunctionUtilities<long>;

namespace Func.Redis.Publisher;

public class RedisPublisherService(ISourcesProvider dbProvider, IRedisSerDes serDes) : IRedisPublisherService
{
    private readonly IDatabase _database = dbProvider.GetDatabase();
    private readonly IRedisSerDes _serDes = serDes;

    public Either<Error, Unit> Publish(string channel, object message) =>
        Wrap(() => _database.Publish(RedisChannel.Literal(channel), _serDes.Serialize(message)), ToUnit);

    public Task<Either<Error, Unit>> PublishAsync(string channel, object message) =>
        WrapAsync(() => _database.PublishAsync(RedisChannel.Literal(channel), _serDes.Serialize(message)), ToUnit);
}