using Func.Redis.SerDes;
using TinyFp.Extensions;

namespace Func.Redis.List;
public class RedisListService(
    ISourcesProvider sourcesProvider,
    IRedisSerDes serDes) : IRedisListService
{
    private readonly IDatabase _database = sourcesProvider.GetDatabase();
    private readonly IRedisSerDes _serDes = serDes;

    public Either<Error, Unit> Prepend<T>(string key, T value) =>
        Try(() => _database.ListLeftPush(key, _serDes.Serialize(value)).Map(_ => Unit.Default))
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Either<Error, Unit> Prepend<T>(string key, params T[] values) =>
        Try(() => _database.ListLeftPush(key, values.Select(_serDes.Serialize).ToArray()).Map(_ => Unit.Default))
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Task<Either<Error, Unit>> PrependAsync<T>(string key, T value) =>
        TryAsync(() => _database.ListLeftPushAsync(key, _serDes.Serialize(value)))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .MapAsync(_ => Unit.Default);

    public Task<Either<Error, Unit>> PrependAsync<T>(string key, params T[] values) =>
        TryAsync(() => _database.ListLeftPushAsync(key, values.Select(_serDes.Serialize).ToArray()))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .MapAsync(_ => Unit.Default);

    public Either<Error, Unit> Append<T>(string key, T value) =>
        Try(() => _database.ListRightPush(key, _serDes.Serialize(value)).Map(_ => Unit.Default))
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Either<Error, Unit> Append<T>(string key, params T[] values) =>
        Try(() => _database.ListRightPush(key, values.Select(_serDes.Serialize).ToArray()))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Map(_ => Unit.Default);

    public Task<Either<Error, Unit>> AppendAsync<T>(string key, T value) =>
        TryAsync(() => _database.ListRightPushAsync(key, _serDes.Serialize(value)))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .MapAsync(_ => Unit.Default);

    public Task<Either<Error, Unit>> AppendAsync<T>(string key, params T[] values) =>
        TryAsync(() => _database.ListRightPushAsync(key, values.Select(_serDes.Serialize).ToArray()))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .MapAsync(_ => Unit.Default);

    public Either<Error, Option<T>> Get<T>(string key, long index) =>
        Try(() => _database.ListGetByIndex(key, index).ToOption(r => r.IsNullOrEmpty).Bind(_serDes.Deserialize<T>))
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Either<Error, Option<T>[]> Get<T>(string key, long start, long stop) =>
        Try(() => _database.ListRange(key, start, stop).Select(_serDes.Deserialize<T>).ToArray())
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Task<Either<Error, Option<T>>> GetAsync<T>(string key, long index) =>
        TryAsync(() => _database.ListGetByIndexAsync(key, index))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(res => Try(() => _serDes.Deserialize<T>(res)).ToEither().MapLeft(e => Error.New(e)));

    public Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, long start, long stop) =>
        TryAsync(() => _database.ListRangeAsync(key, start, stop))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(res => Try(() => res.Select(_serDes.Deserialize<T>).ToArray()).ToEither().MapLeft(e => Error.New(e)));

    public Either<Error, Option<T>> Shift<T>(string key) =>
        Try(() => _database.ListLeftPop(key).ToOption(r => r.IsNullOrEmpty).Bind(_serDes.Deserialize<T>))
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Either<Error, Option<T>[]> Shift<T>(string key, long count) =>
        Try(() => _database.ListLeftPop(key, count).Select(_serDes.Deserialize<T>).ToArray())
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Task<Either<Error, Option<T>>> ShiftAsync<T>(string key) =>
        TryAsync(() => _database.ListLeftPopAsync(key))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(res => Try(() => _serDes.Deserialize<T>(res)).ToEither().MapLeft(e => Error.New(e)));

    public Task<Either<Error, Option<T>[]>> ShiftAsync<T>(string key, long count) =>
        TryAsync(() => _database.ListLeftPopAsync(key, count))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(res => Try(() => res.Select(_serDes.Deserialize<T>).ToArray()).ToEither().MapLeft(e => Error.New(e)));

    public Either<Error, Option<T>> Pop<T>(string key) =>
        Try(() => _database.ListRightPop(key).ToOption(r => r.IsNullOrEmpty).Bind(_serDes.Deserialize<T>))
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Either<Error, Option<T>[]> Pop<T>(string key, long count) =>
        Try(() => _database.ListRightPop(key, count).Select(_serDes.Deserialize<T>).ToArray())
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Task<Either<Error, Option<T>>> PopAsync<T>(string key) =>
        TryAsync(() => _database.ListRightPopAsync(key))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(res => Try(() => _serDes.Deserialize<T>(res)).ToEither().MapLeft(e => Error.New(e)));

    public Task<Either<Error, Option<T>[]>> PopAsync<T>(string key, long count) =>
        TryAsync(() => _database.ListRightPopAsync(key, count))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(res => Try(() => res.Select(_serDes.Deserialize<T>).ToArray()).ToEither().MapLeft(e => Error.New(e)));

    public Either<Error, long> Size(string key) =>
        Try(() => _database.ListLength(key))
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Task<Either<Error, long>> SizeAsync(string key) =>
        TryAsync(() => _database.ListLengthAsync(key))
            .ToEither()
            .MapLeftAsync(e => Error.New(e));
}