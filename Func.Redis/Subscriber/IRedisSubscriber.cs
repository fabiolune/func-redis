namespace Func.Redis.Subscriber;

public interface IRedisSubscriber
{
    /// <summary>
    /// Subscribe to a channel
    /// </summary>
    /// <returns></returns>
    (string, Action<RedisChannel, RedisValue>) GetSubscriptionHandler();
}