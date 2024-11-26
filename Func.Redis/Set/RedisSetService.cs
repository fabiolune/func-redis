using Func.Redis.SerDes;
using TinyFp.Extensions;
using static Func.Redis.Utils.FunctionUtilities;

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
        Wrap(() => _database.SetAdd(key, _serDes.Serialize(value)), SetError);

    public Task<Either<Error, Unit>> AddAsync<T>(string key, T value) =>
        WrapAsync(() => _database.SetAddAsync(key, _serDes.Serialize(value)), SetError);

    public Either<Error, Unit> Delete<T>(string key, T value) =>
        Wrap(() => _database.SetRemove(key, _serDes.Serialize(value)), RemError);

    public Either<Error, Unit> Delete<T>(string key, params T[] values) =>
        Wrap(() => _database.SetRemove(key, values.Select(_serDes.Serialize).ToArray()), _ => Unit.Default);

    public Task<Either<Error, Unit>> DeleteAsync<T>(string key, T value) =>
        WrapAsync(() => _database.SetRemoveAsync(key, _serDes.Serialize(value)), RemError);

    public Task<Either<Error, Unit>> DeleteAsync<T>(string key, params T[] values) =>
        WrapAsync(() => _database.SetRemoveAsync(key, values.Select(_serDes.Serialize).ToArray()), _ => Unit.Default);

    public Either<Error, long> Size(string key) =>
<<<<<<< HEAD
        Wrap(() => _database.SetLength(key));

    public Task<Either<Error, long>> SizeAsync(string key) =>
        WrapAsync(() => _database.SetLengthAsync(key));
=======
        Try(() => _database.SetLength(key))
            .ToEither()
            .MapLeft(e => Error.New(e.Message));

    public Task<Either<Error, long>> SizeAsync(string key) =>
        TryAsync(() => _database.SetLengthAsync(key))
            .ToEither()
            .MapLeftAsync(e => Error.New(e.Message));
>>>>>>> b1c2a9181fe9e03384130d57e1a5ee49976ccc46

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
<<<<<<< HEAD
        Wrap(() => _database.SetCombine(operation, key1, key2).Select(_serDes.Deserialize<T>).Filter().ToArray());
=======
        Try(() => _database.SetCombine(operation, key1, key2))
            .ToEither()
            .MapLeft(e => Error.New(e.Message))
            .Map(values => values.Select(v => _serDes.Deserialize<T>(v)).Filter().ToArray());
>>>>>>> b1c2a9181fe9e03384130d57e1a5ee49976ccc46

    public Either<Error, Option<T>> Pop<T>(string key) =>
        Wrap(() => _database.SetPop(key).Map(_serDes.Deserialize<T>));

    public Task<Either<Error, Option<T>>> PopAsync<T>(string key) =>
<<<<<<< HEAD
        WrapUnsafeAsync(() => _database.SetPopAsync(key), _serDes.Deserialize<T>);

    public Either<Error, Option<T>[]> GetAll<T>(string key) =>
        Wrap(() => _database.SetMembers(key).Select(_serDes.Deserialize<T>).ToArray());

    public Task<Either<Error, Option<T>[]>> GetAllAsync<T>(string key) =>
        WrapUnsafeAsync(() => _database.SetMembersAsync(key), vs => vs.Select(_serDes.Deserialize<T>).ToArray());

    private Task<Either<Error, T[]>> CombineAsync<T>(string key1, string key2, SetOperation operation) =>
        WrapUnsafeAsync(() => _database.SetCombineAsync(operation, key1, key2), vs => vs.Select(_serDes.Deserialize<T>).Filter().ToArray());
=======
        TryAsync(() => _database.SetPopAsync(key))
            .ToEither()
            .MapLeftAsync(e => Error.New(e.Message))
            .BindAsync(rv => Try(() => _serDes.Deserialize<T>(rv)).ToEither().MapLeft(e => Error.New(e.Message)));

    public Either<Error, Option<T>[]> GetAll<T>(string key) =>
        Try(() => _database.SetMembers(key).Select(_serDes.Deserialize<T>).ToArray())
            .ToEither()
            .MapLeft(e => Error.New(e.Message));

    public Task<Either<Error, Option<T>[]>> GetAllAsync<T>(string key) =>
        TryAsync(() => _database.SetMembersAsync(key))
            .ToEither()
            .MapLeftAsync(e => Error.New(e.Message))
            .BindAsync(vs => Try(() => vs.Select(v =>
                                    v.ToOption(v => v.IsNullOrEmpty).Bind(v => _serDes.Deserialize<T>(v))))
                                .Map(o => o.ToArray())
                                .ToEither()
                                .MapLeft(ex => Error.New(ex.Message)));

    private Task<Either<Error, T[]>> CombineAsync<T>(string key1, string key2, SetOperation operation) =>
        TryAsync(() => _database.SetCombineAsync(operation, key1, key2))
            .ToEither()
            .MapLeftAsync(e => Error.New(e.Message))
            .MapAsync(values => values.Select(v => _serDes.Deserialize<T>(v)).Filter().ToArray());
>>>>>>> b1c2a9181fe9e03384130d57e1a5ee49976ccc46
}