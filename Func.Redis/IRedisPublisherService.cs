using TinyFp;

namespace Func.Redis;

public interface IRedisPublisherService
{
    Either<Error, Unit> Publish(string channel, object message);
    Task<Either<Error, Unit>> PublishAsync(string channel, object message);
}