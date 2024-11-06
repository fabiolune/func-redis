using StackExchange.Redis;

namespace Func.Redis;

public interface IRedisSubscriber
{
    (string, Action<RedisChannel, RedisValue>) GetSubscriptionHandler();
}