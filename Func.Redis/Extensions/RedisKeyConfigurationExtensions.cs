using Func.Redis.Models;
using TinyFp;
using TinyFp.Extensions;

namespace Func.Redis.Extensions;

public static class RedisKeyConfigurationExtensions
{
    private const char Colon = ':';

    private static Option<string> GetPrefix(this RedisKeyConfiguration config) =>
        config
            .ToOption(c => c.KeyPrefix is null)
            .Map(c => c.KeyPrefix)
            .Map(p => p.Trim())
            .Map(p => p.Trim(Colon))
            .Bind(p => p.ToOption(string.IsNullOrWhiteSpace));

    public static Func<string, string> GetKeyMapper(this RedisKeyConfiguration config) =>
        config
            .GetPrefix()
            .Match(p => new Func<string, string>(key => $"{p}{Colon}{key}"), () => key => key);

    public static Func<string, string> GetInverseKeyMapper(this RedisKeyConfiguration config) =>
        config
            .GetPrefix()
            .Match(p => new Func<string, string>(key =>
                key
                    .ToOption(k => !k.StartsWith(p))
                    .Match(k => k[(p.Length + 1)..], () => key)
            ), () => key => key);
}