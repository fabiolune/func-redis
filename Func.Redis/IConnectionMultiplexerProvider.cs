namespace Func.Redis;

public interface IConnectionMultiplexerProvider
{
    IConnectionMultiplexer GetMultiplexer();
}