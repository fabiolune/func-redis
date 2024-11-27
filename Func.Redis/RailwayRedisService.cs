using static Func.Redis.Utils.FunctionUtilities;

namespace Func.Redis;

internal class RailwayRedisService(ISourcesProvider provider) : IRedisService
{
    private readonly IDatabase _database = provider.GetDatabase();

    public Either<Error, T> Execute<T>(Func<IDatabase, T> exec) =>
        Wrap(() => exec(_database));

    public Either<Error, TOut> Execute<TIn, TOut>(Func<IDatabase, TIn> exec, Func<TIn, TOut> map) =>
        Wrap(() => exec(_database), map);

    public Task<Either<Error, T>> ExecuteAsync<T>(Func<IDatabase, Task<T>> exec) =>
        WrapAsync(() => exec(_database));

    public Task<Either<Error, TOut>> ExecuteAsync<TIn, TOut>(Func<IDatabase, Task<TIn>> exec, Func<TIn, TOut> map) =>
        WrapAsync(() => exec(_database), map);

    public Task<Either<Error, TOut>> ExecuteUnsafeAsync<TIn, TOut>(Func<IDatabase, Task<TIn>> exec, Func<TIn, TOut> map) =>
        WrapUnsafeAsync(() => exec(_database), map);
}