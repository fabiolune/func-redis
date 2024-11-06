using System.Diagnostics.CodeAnalysis;

namespace Func.Redis.Models;

[ExcludeFromCodeCoverage]
public record RedisKeyConfiguration
{
    public string KeyPrefix { get; init; } = string.Empty;
}
