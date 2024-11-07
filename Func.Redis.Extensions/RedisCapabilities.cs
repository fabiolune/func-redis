namespace Func.Redis.Extensions;

[Flags]
public enum RedisCapabilities
{
    Keys = 1,
    HashSet = 2,
    Publisher = 4,
    Subscriber = 8
}