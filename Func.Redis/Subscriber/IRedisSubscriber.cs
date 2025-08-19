namespace Func.Redis.Subscriber;

public interface IRedisSubscriber
{
    /// <summary>
    /// Retrieves the subscription handler for processing Redis messages.
    /// </summary>
    /// <returns>A tuple containing the subscription channel name as a <see cref="string"/>  and the callback <see
    /// cref="Action{RedisChannel, RedisValue}"/> to handle incoming messages.</returns>
    (string, Action<RedisChannel, RedisValue>) GetSubscriptionHandler();
}