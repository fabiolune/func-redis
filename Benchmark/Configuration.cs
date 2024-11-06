using Sport.Redis.Models;
using System.Diagnostics.CodeAnalysis;

namespace Benchmark;

[ExcludeFromCodeCoverage]
public static class Configuration
{
    public static readonly RedisConfiguration Config = new()
    {
        ConnectionString = "localhost:6379"
    };

    public static readonly RedisKeyConfiguration KeyConfig = new()
    {
        KeyPrefix = string.Empty
    };
}