using SpanJson;
using TinyFp.Extensions;

namespace Func.Redis.SerDes.Json;

public class SpanJsonRedisSerDes : IRedisSerDes
{
    public Option<T> Deserialize<T>(RedisValue value) =>
        value
            .ToOption(v => v.IsNullOrEmpty)
            .Bind(v => JsonSerializer.Generic.Utf16.Deserialize<T>(v).ToOption())
            .Map(arg => arg!);

    public Option<object> Deserialize(RedisValue value, Type type) =>
        value
            .ToOption(v => v.IsNullOrEmpty)
            .Bind(v => JsonSerializer.NonGeneric.Utf16.Deserialize(((string)v!).AsSpan(), type).ToOption())
            .Map(o => o!);

    public Option<T[]> Deserialize<T>(RedisValue[] values) =>
        values
            .ToOption(vs => vs.Length == 0 || Array.Exists(vs, v => v.IsNullOrEmpty || v.ToString() == JsonConstants.NullJson))
            .Map(vs => vs.Select(v => JsonSerializer.Generic.Utf16.Deserialize<T>(v)!))
            .Map(_ => _.ToArray());

    public Option<(string, T)[]> Deserialize<T>(HashEntry[] entries) =>
        entries
            .ToOption(hs => hs.Length == 0 || Array.Exists(hs, h => h.Value.IsNullOrEmpty || h.Value.ToString() == JsonConstants.NullJson))
            .Map(hs => hs.Select(h => (h.Name.ToString(), JsonSerializer.Generic.Utf16.Deserialize<T>(h.Value)!)))
            .Map(ts => ts.ToArray());

    public RedisValue Serialize<T>(T value) =>
        JsonSerializer.Generic.Utf16
            .Serialize(value);
}