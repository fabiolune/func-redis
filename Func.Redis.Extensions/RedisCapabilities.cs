namespace Func.Redis.Extensions;

[Flags]
public enum RedisCapabilities
{
    Keys = 1,
    HashSet = 2,
    Set = 4,
    Publisher = 8,
    Subscriber = 16
}