namespace Func.Redis.Extensions;

[Flags]
public enum RedisCapabilities
{
    Generic     = 1 << 0,
    Keys        = 1 << 1,
    HashSet     = 1 << 2,
    Set         = 1 << 3,
    List        = 1 << 4,
    Publisher   = 1 << 5,
    Subscriber  = 1 << 6
}