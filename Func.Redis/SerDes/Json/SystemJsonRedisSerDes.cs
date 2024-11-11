using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;
using TinyFp;
using TinyFp.Extensions;

namespace Func.Redis.SerDes.Json;

public class SystemJsonRedisSerDes : IRedisSerDes
{
    private static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public Option<T> Deserialize<T>(RedisValue value) =>
        value
            .ToOption(v => v == RedisValue.Null || v == RedisValue.EmptyString)
            .Bind(v => JsonSerializer.Deserialize<T>(v!, CaseInsensitiveOptions).ToOption())
            .Map(arg => arg!);

    public Option<object> Deserialize(RedisValue value, Type type) =>
        value
            .ToOption(v => v == RedisValue.Null || v == RedisValue.EmptyString)
            .Bind(v => JsonSerializer.Deserialize(v!, type, CaseInsensitiveOptions).ToOption())
            .Map(o => o!);

    public Option<T[]> Deserialize<T>(RedisValue[] values) =>
        values
            .ToOption(vs => vs.Length == 0 || Array.Exists(vs, v => v == RedisValue.Null || v == RedisValue.EmptyString || v.ToString() == JsonConstants.NullJson))
            .Map(vs => vs.Select(v => JsonSerializer.Deserialize<T>(v!, CaseInsensitiveOptions)!))
            .Map(_ => _.ToArray());

    public Option<(string, T)[]> Deserialize<T>(HashEntry[] entries) =>
        entries
            .ToOption(hs => hs.Length == 0 || Array.Exists(hs, h => h.Value == RedisValue.Null || h.Value == RedisValue.EmptyString || h.Value.ToString() == JsonConstants.NullJson))
            .Map(hs => hs.Select(h => (h.Name.ToString(), JsonSerializer.Deserialize<T>(h.Value!, CaseInsensitiveOptions)!)))
            .Map(ts => ts.ToArray());

    public RedisValue Serialize<T>(T value) =>
        JsonSerializer
            .Serialize(value, CaseInsensitiveOptions);

}