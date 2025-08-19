namespace Func.Redis.Publisher;

public interface IRedisPublisherService
{
    /// <summary>
    /// Publishes a message to the specified channel.
    /// </summary>
    /// <remarks>This method allows sending messages to a specific channel, which can be used for
    /// communication or event propagation. Ensure that the channel name and message are valid and conform to the
    /// expected format for the underlying system.</remarks>
    /// <param name="channel">The name of the channel to which the message will be published. Cannot be null or empty.</param>
    /// <param name="message">The message to be published. Typically an object representing the data to send. Cannot be null.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see langword="Unit"/> if
    /// the message is successfully published, or an <see cref="Error"/> if the operation fails.</returns>
    Either<Error, Unit> Publish(string channel, object message);
    
    /// <inheritdoc cref="Publish(string, object)"/>
    Task<Either<Error, Unit>> PublishAsync(string channel, object message);
}