namespace Func.Redis.Extensions;

[Flags]
public enum RedisCapabilities
{
    Generic = 1 << 0,
    Key = 1 << 1,
    HashSet = 1 << 2,
    Set = 1 << 3,
    SortedSet = 1 << 4,
    List = 1 << 5,
    Publish = 1 << 6,
    Subscribe = 1 << 7
}