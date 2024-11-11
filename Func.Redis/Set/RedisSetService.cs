using Func.Redis.SerDes;
using StackExchange.Redis;
using TinyFp;
using TinyFp.Extensions;
using static TinyFp.Prelude;

namespace Func.Redis.Set;
public class RedisSetService(
    ISourcesProvider sourcesProvider,
    IRedisSerDes serDes) : IRedisSetService
{
    private static readonly Error SetError = Error.New("Redis SADD Error");
    private static readonly Error RemError = Error.New("Redis SREM Error");

    private readonly IDatabase _database = sourcesProvider.GetDatabase();
    private readonly IRedisSerDes _serDes = serDes;

    public Either<Error, Unit> Add<T>(string key, T value) =>
        Try(() => _database.SetAdd(key, _serDes.Serialize(value)))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Bind(res => res.ToEither(_ => Unit.Default, b => !b, SetError));

    public Task<Either<Error, Unit>> AddAsync<T>(string key, T value) =>
        TryAsync(() => _database.SetAddAsync(key, _serDes.Serialize(value)))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(res => res.ToEither(_ => Unit.Default, b => !b, SetError));

    public Either<Error, Unit> Delete<T>(string key, T value) =>
        Try(() => _database.SetRemove(key, _serDes.Serialize(value)))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Bind(res => res.ToEither(_ => Unit.Default, b => !b, RemError));

    public Either<Error, Unit> Delete<T>(string key, params T[] values) =>
        Try(() => _database.SetRemove(key, values.Select(_serDes.Serialize).ToArray()))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Map(_ => Unit.Default);

    public Task<Either<Error, Unit>> DeleteAsync<T>(string key, T value) =>
        TryAsync(() => _database.SetRemoveAsync(key, _serDes.Serialize(value)))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(res => res.ToEither(_ => Unit.Default, b => !b, RemError));

    public Task<Either<Error, Unit>> DeleteAsync<T>(string key, params T[] values) =>
        TryAsync(() => _database.SetRemoveAsync(key, values.Select(_serDes.Serialize).ToArray()))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .MapAsync(_ => Unit.Default);

    public Either<Error, long> Size(string key) =>
        Try(() => _database.SetLength(key))
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Task<Either<Error, long>> SizeAsync(string key) =>
        TryAsync(() => _database.SetLengthAsync(key))
            .ToEither()
            .MapLeftAsync(e => Error.New(e));

    public Either<Error, T[]> Intersect<T>(string key1, string key2) =>
        Combine<T>(key1, key2, SetOperation.Intersect);

    public Task<Either<Error, T[]>> IntersectAsync<T>(string key1, string key2) =>
        CombineAsync<T>(key1, key2, SetOperation.Intersect);

    public Either<Error, T[]> Union<T>(string key1, string key2) =>
        Combine<T>(key1, key2, SetOperation.Union);

    public Task<Either<Error, T[]>> UnionAsync<T>(string key1, string key2) =>
        CombineAsync<T>(key1, key2, SetOperation.Union);

    public Either<Error, T[]> Difference<T>(string key1, string key2) =>
        Combine<T>(key1, key2, SetOperation.Difference);

    public Task<Either<Error, T[]>> DifferenceAsync<T>(string key1, string key2) =>
        CombineAsync<T>(key1, key2, SetOperation.Difference);

    private Either<Error, T[]> Combine<T>(string key1, string key2, SetOperation operation) =>
        Try(() => _database.SetCombine(operation, key1, key2))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Map(values => values.Select(v => _serDes.Deserialize<T>(v)).Filter().ToArray());

    public Either<Error, Option<T>> Pop<T>(string key) =>
        Try(() => _database.SetPop(key))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Bind(rv => Try(() => _serDes.Deserialize<T>(rv)).ToEither().MapLeft(e => Error.New(e)));

    public Task<Either<Error, Option<T>>> PopAsync<T>(string key) =>
        TryAsync(() => _database.SetPopAsync(key))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(rv => Try(() => _serDes.Deserialize<T>(rv)).ToEither().MapLeft(e => Error.New(e)));

    public Either<Error, Option<T>[]> GetAll<T>(string key) =>
        Try(() => _database.SetMembers(key))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Bind(vs => Try(() => vs.Select(value =>
                               value.ToOption(v => !v.HasValue)
                                   .Bind(v => _serDes.Deserialize<T>(v))))
                           .Map(o => o.ToArray())
                           .ToEither()
                           .MapLeft(ex => Error.New(ex.Message)));

    public Task<Either<Error, Option<T>[]>> GetAllAsync<T>(string key) =>
        TryAsync(() => _database.SetMembersAsync(key))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(vs => Try(() => vs.Select(v =>
                                    v.ToOption(v => !v.HasValue)
                                        .Bind(v => _serDes.Deserialize<T>(v))))
                                .Map(o => o.ToArray())
                                .ToEither()
                                .MapLeft(ex => Error.New(ex.Message)));

    private Task<Either<Error, T[]>> CombineAsync<T>(string key1, string key2, SetOperation operation) =>
        TryAsync(() => _database.SetCombineAsync(operation, key1, key2))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .MapAsync(values => values.Select(v => _serDes.Deserialize<T>(v)).Filter().ToArray());
}