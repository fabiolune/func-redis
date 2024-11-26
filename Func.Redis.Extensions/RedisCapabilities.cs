namespace Func.Redis.Extensions;

[Flags]
public enum RedisCapabilities
{
    Keys        = 1 << 0,
    HashSet     = 1 << 1,
    Set         = 1 << 2,
    List        = 1 << 3,
    Publisher   = 1 << 4,
    Subscriber  = 1 << 5
}