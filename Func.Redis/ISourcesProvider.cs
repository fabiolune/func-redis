namespace Func.Redis;

public interface ISourcesProvider
{
    IDatabase GetDatabase();
    IServer[] GetServers();
}