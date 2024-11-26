using Func.Redis.SerDes;
using Func.Redis.Utils;
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
        Wrap(() => _database.SetRemove(key, values.Select(_serDes.Serialize).ToArray()), FunctionUtilities<long>.ToUnit);

    public Task<Either<Error, Unit>> DeleteAsync<T>(string key, T value) =>
        WrapAsync(() => _database.SetRemoveAsync(key, _serDes.Serialize(value)), RemError);

    public Task<Either<Error, Unit>> DeleteAsync<T>(string key, params T[] values) =>
        WrapAsync(() => _database.SetRemoveAsync(key, values.Select(_serDes.Serialize).ToArray()), FunctionUtilities<long>.ToUnit);

    public Either<Error, long> Size(string key) =>
        Wrap(() => _database.SetLength(key));

    public Task<Either<Error, long>> SizeAsync(string key) =>
        WrapAsync(() => _database.SetLengthAsync(key));

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
        WrapUnsafe(() => _database.SetCombine(operation, key1, key2), res => res.Select(_serDes.Deserialize<T>).Filter().ToArray());

    public Either<Error, Option<T>> Pop<T>(string key) =>
        WrapUnsafe(() => _database.SetPop(key), _serDes.Deserialize<T>);

    public Task<Either<Error, Option<T>>> PopAsync<T>(string key) =>
        WrapUnsafeAsync(() => _database.SetPopAsync(key), _serDes.Deserialize<T>);

    public Either<Error, Option<T>[]> GetAll<T>(string key) =>
        WrapUnsafe(() => _database.SetMembers(key), res => res.Select(_serDes.Deserialize<T>).ToArray());

    public Task<Either<Error, Option<T>[]>> GetAllAsync<T>(string key) =>
        WrapUnsafeAsync(() => _database.SetMembersAsync(key), vs => vs.Select(_serDes.Deserialize<T>).ToArray());

    private Task<Either<Error, T[]>> CombineAsync<T>(string key1, string key2, SetOperation operation) =>
        WrapUnsafeAsync(() => _database.SetCombineAsync(operation, key1, key2), vs => vs.Select(_serDes.Deserialize<T>).Filter().ToArray());
}