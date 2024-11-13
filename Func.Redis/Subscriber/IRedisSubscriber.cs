namespace Func.Redis.Subscriber;

public interface IRedisSubscriber
{
    (string, Action<RedisChannel, RedisValue>) GetSubscriptionHandler();
}