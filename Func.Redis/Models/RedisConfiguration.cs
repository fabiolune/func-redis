using System.Diagnostics.CodeAnalysis;

namespace Func.Redis.Models;

[ExcludeFromCodeCoverage]
public record RedisConfiguration
{
    public string ConnectionString { get; init; } = string.Empty;
}