using StackExchange.Redis;

namespace Func.Redis;

public interface ISourcesProvider
{
    IDatabase GetDatabase();
    IServer[] GetServers();
}