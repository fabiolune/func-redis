namespace Func.Redis;

public interface IConnectionMultiplexerProvider
{
    /// <summary>
    /// Retrieves the current instance of the connection multiplexer used to manage Redis connections.
    /// </summary>
    /// <returns>An instance of <see cref="IConnectionMultiplexer"/> representing the connection multiplexer.</returns>
    IConnectionMultiplexer GetMultiplexer();
}