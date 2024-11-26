using Func.Redis.SerDes;
using TinyFp.Extensions;
using static Func.Redis.Utils.FunctionUtilities;

namespace Func.Redis.List;
public class RedisListService(
    ISourcesProvider sourcesProvider,
    IRedisSerDes serDes) : IRedisListService
{
    private readonly IDatabase _database = sourcesProvider.GetDatabase();
    private readonly IRedisSerDes _serDes = serDes;

    public Either<Error, Unit> Prepend<T>(string key, T value) =>
        Wrap(() => _database.ListLeftPush(key, _serDes.Serialize(value)), _ => Unit.Default);

    public Either<Error, Unit> Prepend<T>(string key, params T[] values) =>
        Wrap(() => _database.ListLeftPush(key, values.Select(_serDes.Serialize).ToArray()), _ => Unit.Default);

    public Task<Either<Error, Unit>> PrependAsync<T>(string key, T value) =>
        WrapAsync(() => _database.ListLeftPushAsync(key, _serDes.Serialize(value)), _ => Unit.Default);

    public Task<Either<Error, Unit>> PrependAsync<T>(string key, params T[] values) =>
        WrapAsync(() => _database.ListLeftPushAsync(key, values.Select(_serDes.Serialize).ToArray()), _ => Unit.Default);

    public Either<Error, Unit> Append<T>(string key, T value) =>
        Wrap(() => _database.ListRightPush(key, _serDes.Serialize(value)), _ => Unit.Default);

    public Either<Error, Unit> Append<T>(string key, params T[] values) =>
        Wrap(() => _database.ListRightPush(key, values.Select(_serDes.Serialize).ToArray()), _ => Unit.Default);

    public Task<Either<Error, Unit>> AppendAsync<T>(string key, T value) =>
        WrapAsync(() => _database.ListRightPushAsync(key, _serDes.Serialize(value)), _ => Unit.Default);

    public Task<Either<Error, Unit>> AppendAsync<T>(string key, params T[] values) =>
        WrapAsync(() => _database.ListRightPushAsync(key, values.Select(_serDes.Serialize).ToArray()), _ => Unit.Default);

    public Either<Error, Option<T>> Get<T>(string key, long index) =>
        Wrap(() => _database.ListGetByIndex(key, index).ToOption(r => r.IsNullOrEmpty).Bind(_serDes.Deserialize<T>));

    public Either<Error, Option<T>[]> Get<T>(string key, long start, long stop) =>
        Wrap(() => _database.ListRange(key, start, stop).Select(_serDes.Deserialize<T>).ToArray());

    public Task<Either<Error, Option<T>>> GetAsync<T>(string key, long index) =>
        WrapUnsafeAsync(() => _database.ListGetByIndexAsync(key, index), _serDes.Deserialize<T>);

    public Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, long start, long stop) =>
        WrapUnsafeAsync(() => _database.ListRangeAsync(key, start, stop), res => res.Select(_serDes.Deserialize<T>).ToArray());

    public Either<Error, Option<T>> Shift<T>(string key) =>
        Wrap(() => _database.ListLeftPop(key).Map(_serDes.Deserialize<T>));

    public Either<Error, Option<T>[]> Shift<T>(string key, long count) =>
        Wrap(() => _database.ListLeftPop(key, count).Select(_serDes.Deserialize<T>).ToArray());

    public Task<Either<Error, Option<T>>> ShiftAsync<T>(string key) =>
        WrapUnsafeAsync(() => _database.ListLeftPopAsync(key), _serDes.Deserialize<T>);

    public Task<Either<Error, Option<T>[]>> ShiftAsync<T>(string key, long count) =>
        WrapUnsafeAsync(() => _database.ListLeftPopAsync(key, count), r => r.Select(_serDes.Deserialize<T>).ToArray());

    public Either<Error, Option<T>> Pop<T>(string key) =>
        Wrap(() => _database.ListRightPop(key).ToOption(r => r.IsNullOrEmpty).Bind(_serDes.Deserialize<T>));

    public Either<Error, Option<T>[]> Pop<T>(string key, long count) =>
        Wrap(() => _database.ListRightPop(key, count).Select(_serDes.Deserialize<T>).ToArray());

    public Task<Either<Error, Option<T>>> PopAsync<T>(string key) =>
        WrapUnsafeAsync(() => _database.ListRightPopAsync(key), _serDes.Deserialize<T>);

    public Task<Either<Error, Option<T>[]>> PopAsync<T>(string key, long count) =>
        WrapUnsafeAsync(() => _database.ListRightPopAsync(key, count), r => r.Select(_serDes.Deserialize<T>).ToArray());

    public Either<Error, long> Size(string key) =>
        Wrap(() => _database.ListLength(key));

    public Task<Either<Error, long>> SizeAsync(string key) =>
        WrapAsync(() => _database.ListLengthAsync(key));
}