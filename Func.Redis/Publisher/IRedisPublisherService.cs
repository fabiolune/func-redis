namespace Func.Redis.Publisher;

public interface IRedisPublisherService
{
    /// <summary>
    /// Publish a message to a channel
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    Either<Error, Unit> Publish(string channel, object message);
    /// <inheritdoc cref="Publish(string, object)"/>
    Task<Either<Error, Unit>> PublishAsync(string channel, object message);
}