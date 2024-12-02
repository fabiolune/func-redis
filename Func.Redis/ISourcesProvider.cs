namespace Func.Redis;
public interface ISourcesProvider
{
    /// <summary>
    /// Get the database instance of the Redis server
    /// </summary>
    /// <returns></returns>
    IDatabase GetDatabase();
    /// <summary>
    /// Get all the server references of the Redis instance
    /// </summary>
    /// <returns></returns>
    IServer[] GetServers();
}
