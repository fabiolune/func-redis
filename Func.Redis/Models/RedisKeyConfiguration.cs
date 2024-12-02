using System.Diagnostics.CodeAnalysis;

namespace Func.Redis.Models;

[ExcludeFromCodeCoverage]
public record RedisKeyConfiguration
{
    /// <summary>
    /// The prefix to be added to the key
    /// </summary>
    public string KeyPrefix { get; init; } = string.Empty;
}
