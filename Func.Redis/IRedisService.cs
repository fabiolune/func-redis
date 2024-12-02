
namespace Func.Redis;

public interface IRedisService
{
    /// <summary>
    /// Execute a Redis command
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="exec"></param>
    /// <returns></returns>
    Either<Error, T> Execute<T>(Func<IDatabase, T> exec);
    /// <summary>
    /// Execute a Redis command and map the result
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="exec"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    Either<Error, TOut> Execute<TIn, TOut>(Func<IDatabase, TIn> exec, Func<TIn, TOut> map);
    /// <inheritdoc cref="Execute{T}(Func{IDatabase, T})"/>/>
    Task<Either<Error, T>> ExecuteAsync<T>(Func<IDatabase, Task<T>> exec);
    /// <inheritdoc cref="Execute{TIn, TOut}(Func{IDatabase, TIn}, Func{TIn, TOut})"/>
    Task<Either<Error, TOut>> ExecuteAsync<TIn, TOut>(Func<IDatabase, Task<TIn>> exec, Func<TIn, TOut> map);
    /// <summary>
    /// Execute a Redis command and map the result. This method explicitely catches exceptions of the map Func
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="exec"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    Task<Either<Error, TOut>> ExecuteUnsafeAsync<TIn, TOut>(Func<IDatabase, Task<TIn>> exec, Func<TIn, TOut> map);
}