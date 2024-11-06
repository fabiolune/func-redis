using StackExchange.Redis;
using TinyFp;

namespace Func.Redis.SerDes;

public interface IRedisSerDes
{
    Option<T> Deserialize<T>(RedisValue value);
    Option<T[]> Deserialize<T>(RedisValue[] values);
    Option<(string, T)[]> Deserialize<T>(HashEntry[] entries);
    Option<object> Deserialize(RedisValue value, Type type);
    RedisValue Serialize<T>(T value);
}