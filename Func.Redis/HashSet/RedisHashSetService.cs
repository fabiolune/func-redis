using Func.Redis.SerDes;
using Func.Redis.Utils;
using TinyFp.Extensions;
using static Func.Redis.Utils.FunctionUtilities;

namespace Func.Redis.HashSet;

public class RedisHashSetService(
    ISourcesProvider dbProvider,
    IRedisSerDes serDes) : IRedisHashSetService
{
    private readonly IDatabase _database = dbProvider.GetDatabase();
    private readonly IRedisSerDes _serDes = serDes;

    public Either<Error, Unit> Delete(string key, string field) =>
        Wrap(() => _database.HashDelete(key, field), FunctionUtilities<bool>.ToUnit);

    public Either<Error, Unit> Delete(string key, params string[] fields) =>
        Wrap(() => _database.HashDelete(key, fields.Map(field => (RedisValue)field).ToArray()), FunctionUtilities<long>.ToUnit);

    public Task<Either<Error, Unit>> DeleteAsync(string key, string field) =>
        WrapAsync(() => _database.HashDeleteAsync(key, field), FunctionUtilities<bool>.ToUnit);

    public Task<Either<Error, Unit>> DeleteAsync(string key, params string[] fields) =>
        WrapAsync(() => _database.HashDeleteAsync(key, fields.Map(field => (RedisValue)field).ToArray()), FunctionUtilities<long>.ToUnit);

    public Either<Error, Option<T>> Get<T>(string key, string field) =>
        WrapUnsafe(() => _database.HashGet(key, field), _serDes.Deserialize<T>);

    public Either<Error, Option<T>[]> Get<T>(string key, params string[] fields) =>
        WrapUnsafe(() => _database.HashGet(key, fields.Map(field => (RedisValue)field).ToArray()), res => res.Select(_serDes.Deserialize<T>).ToArray());

    public Either<Error, Option<object>[]> Get(string key, params (Type, string)[] typeFields) =>
        WrapUnsafe(() => _database.HashGet(key, typeFields.Select(tf => (RedisValue)tf.Item2).ToArray()),
            rvt => rvt.Zip(typeFields.Select(tf => tf.Item1)).Select(t => _serDes.Deserialize(t.First!, t.Second)).ToArray());

    public Task<Either<Error, Option<T>>> GetAsync<T>(string key, string field) =>
        WrapUnsafeAsync(() => _database.HashGetAsync(key, field), _serDes.Deserialize<T>);

    public Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, params string[] fields) =>
        WrapUnsafeAsync(() => _database.HashGetAsync(key, fields.Select(f => (RedisValue)f).ToArray()), vs => vs.Select(_serDes.Deserialize<T>).ToArray());

    public Task<Either<Error, Option<object>[]>> GetAsync(string key, params (Type, string)[] typeFields) =>
        WrapUnsafeAsync(() => _database.HashGetAsync(key, typeFields.Select(tf => (RedisValue)tf.Item2).ToArray()),
            rvt => rvt.Zip(typeFields.Select(tf => tf.Item1)).Select(t => _serDes.Deserialize(t.First!, t.Second)).ToArray());

    public Either<Error, Unit> Set<T>(string key, string field, T value) =>
        Wrap(() => _database.HashSet(key, field, _serDes.Serialize(value)), FunctionUtilities<bool>.ToUnit);

    public Either<Error, Unit> Set<T>(string key, params (string, T)[] pairs) =>
        Wrap(() => Unit.Default.Tee(_ => _database.HashSet(key, pairs.Select(t => new HashEntry(t.Item1, _serDes.Serialize(t.Item2))).ToArray())));

    public Task<Either<Error, Unit>> SetAsync<T>(string key, string field, T value) =>
        WrapAsync(() => _database.HashSetAsync(key, field, _serDes.Serialize(value)), FunctionUtilities<bool>.ToUnit);

    public Task<Either<Error, Unit>> SetAsync<T>(string key, params (string, T)[] pairs) =>
        WrapAsync(() => _database.HashSetAsync(key, pairs.Select(t => new HashEntry(t.Item1, _serDes.Serialize(t.Item2))).ToArray()).ToTaskUnit<object>());

    public Either<Error, Option<T[]>> GetValues<T>(string key) =>
        WrapUnsafe(() => _database.HashValues(key), _serDes.Deserialize<T>);

    public Task<Either<Error, Option<T[]>>> GetValuesAsync<T>(string key) =>
        WrapUnsafeAsync(() => _database.HashValuesAsync(key), _serDes.Deserialize<T>);

    public Either<Error, Option<(string, T)[]>> GetAll<T>(string key) =>
        WrapUnsafe(() => _database.HashGetAll(key), _serDes.Deserialize<T>);

    public Task<Either<Error, Option<(string, T)[]>>> GetAllAsync<T>(string key) =>
        WrapUnsafeAsync(() => _database.HashGetAllAsync(key), _serDes.Deserialize<T>);

    public Either<Error, Option<string[]>> GetFieldKeys(string key) =>
        WrapUnsafe(() => _database.HashKeys(key), vv => vv.ToOption(v => v.Length == 0).Map(v => v.Select(x => x.ToString()!).ToArray()));

    public Task<Either<Error, Option<string[]>>> GetFieldKeysAsync(string key) =>
        WrapUnsafeAsync(() => _database.HashKeysAsync(key), res => res.ToOption(v => v.Length == 0).Map(v => v.Select(x => x.ToString()!).ToArray()));
}