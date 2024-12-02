namespace Func.Redis;

public interface IConnectionMultiplexerProvider
{
    /// <summary>
    /// Get the connection multiplexer
    /// </summary>
    /// <returns></returns>
    IConnectionMultiplexer GetMultiplexer();
}