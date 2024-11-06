using StackExchange.Redis;

namespace Func.Redis;

public interface IConnectionMultiplexerProvider
{
    IConnectionMultiplexer GetMultiplexer();
}