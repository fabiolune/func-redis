namespace Func.Redis;
public interface ISourcesProvider
{
    /// <summary>
    /// Retrieves an instance of the database interface for performing operations.
    /// </summary>
    /// <returns>An object implementing the <see cref="IDatabase"/> interface, which provides methods for interacting with the
    /// database.</returns>
    IDatabase GetDatabase();

    /// <summary>
    /// Retrieves an array of servers currently available in the system.
    /// </summary>
    /// <returns>An array of <see cref="IServer"/> objects representing the available servers.      The array will be empty if no
    /// servers are available.</returns>
    IServer[] GetServers();
}
