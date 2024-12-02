using System.Diagnostics.CodeAnalysis;

namespace Func.Redis.Models;

[ExcludeFromCodeCoverage]
public record RedisConfiguration
{
    /// <summary>
    /// The connection string to the Redis server
    /// </summary>
    public string ConnectionString { get; init; } = string.Empty;
}