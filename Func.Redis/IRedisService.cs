
namespace Func.Redis;

public interface IRedisService
{
    Either<Error, T> Execute<T>(Func<IDatabase, T> exec);
    Either<Error, TOut> Execute<TIn, TOut>(Func<IDatabase, TIn> exec, Func<TIn, TOut> map);
    Task<Either<Error, T>> ExecuteAsync<T>(Func<IDatabase, Task<T>> exec);
    Task<Either<Error, TOut>> ExecuteAsync<TIn, TOut>(Func<IDatabase, Task<TIn>> exec, Func<TIn, TOut> map);
    Task<Either<Error, TOut>> ExecuteUnsafeAsync<TIn, TOut>(Func<IDatabase, Task<TIn>> exec, Func<TIn, TOut> map);
}