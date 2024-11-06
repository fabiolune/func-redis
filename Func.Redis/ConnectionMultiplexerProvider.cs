using Func.Redis.Models;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace Func.Redis;

[ExcludeFromCodeCoverage]
public class ConnectionMultiplexerProvider(RedisConfiguration config) : IConnectionMultiplexerProvider
{
    private readonly ConnectionMultiplexer _mux = ConnectionMultiplexer.Connect(config.ConnectionString);

    public IConnectionMultiplexer GetMultiplexer() => _mux;
}