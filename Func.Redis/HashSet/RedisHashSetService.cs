using Func.Redis.SerDes;
using StackExchange.Redis;
using TinyFp;
using TinyFp.Extensions;
using static TinyFp.Prelude;
namespace Func.Redis.HashSet;

public class RedisHashSetService(
    ISourcesProvider dbProvider,
    IRedisSerDes serDes) : IRedisHashSetService
{
    private readonly IDatabase _database = dbProvider.GetDatabase();
    private readonly IRedisSerDes _serDes = serDes;

    public Either<Error, Unit> Delete(string key, string field) =>
        Try(() =>
            _database.HashDelete(key, field))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Map(_ => Unit.Default);

    public Either<Error, Unit> Delete(string key, params string[] fields)
        => Try(() => _database
                .HashDelete(key, fields.Map(field => (RedisValue)field).ToArray()))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Map(_ => Unit.Default);

    public Task<Either<Error, Unit>> DeleteAsync(string key, string field) =>
        TryAsync(() =>
            _database.HashDeleteAsync(key, field))
            .ToEither().MapLeftAsync(e => Error.New(e))
            .MapAsync(_ => Unit.Default);

    public Task<Either<Error, Unit>> DeleteAsync(string key, params string[] fields) =>
        TryAsync(() =>
            _database.HashDeleteAsync(key, fields.Map(field => (RedisValue)field).ToArray()))
            .ToEither().MapLeftAsync(e => Error.New(e))
            .MapAsync(_ => Unit.Default);

    public Either<Error, Option<T>> Get<T>(string key, string field) =>
        Try(() =>
            _database.HashGet(key, field))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Map(v => v.ToOption(_ => !_.HasValue))
            .Bind(v => v.Match(_ =>
                Try(() => _serDes.Deserialize<T>(_))
                .ToEither()
                .MapLeft(e => Error.New(e)),
            () => Option<T>.None()));

    public Either<Error, Option<T>[]> Get<T>(string key, params string[] fields) =>
        Try(() =>
            _database.HashGet(key, fields.Map(field => (RedisValue)field).ToArray()))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Map(rv =>
                rv.Select(value => value
                        .ToOption(v => v.IsNullOrEmpty)
                        .Bind(v => Try(() => _serDes.Deserialize<T>(v))
                        .OnFail(_ => Option<T>.None())))
                    .ToArray());

    public Either<Error, Option<object>[]> Get(string key, params (Type, string)[] typeFields) =>
        Try(() => _database.HashGet(key, typeFields.Select(tf => (RedisValue)tf.Item2).ToArray()))
            .Map(rvs => rvs.Zip(typeFields.Select(tf => tf.Item1)))
            .ToEither().MapLeft(e => Error.New(e))
            .Map(rvt => rvt.Select(r => Try(() => _serDes.Deserialize(r.First!, r.Second))
                .OnFail(_ => Option<object>.None())).ToArray());

    public Task<Either<Error, Option<T>>> GetAsync<T>(string key, string field) =>
        TryAsync(() =>
            _database.HashGetAsync(key, field))
            .ToEither().MapLeftAsync(e => Error.New(e))
            .MapAsync(value => value.ToOption(v => v.IsNullOrEmpty))
            .BindAsync(v => v.Match(_ =>
                Try(() => _serDes.Deserialize<T>(_!)).ToEither().MapLeft(e => Error.New(e)),
                () => Option<T>.None()));

    public Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, params string[] fields) =>
        TryAsync(() => _database.HashGetAsync(key, fields.Select(f => (RedisValue)f).ToArray()))
        .ToEither().MapLeftAsync(e => Error.New(e))
        .MapAsync(vs => vs
            .Select(rv => rv.ToOption(_ => _.IsNullOrEmpty)
                .Bind(ttt =>
                    Try(() => _serDes.Deserialize<T>(ttt!))
                    .OnFail(_ => Option<T>.None())))
            .ToArray());

    public Task<Either<Error, Option<object>[]>> GetAsync(string key, params (Type, string)[] typeFields) =>
        TryAsync(() => _database.HashGetAsync(key, typeFields.Select(tf => (RedisValue)tf.Item2).ToArray()))
            .ToEither().MapLeftAsync(e => Error.New(e))
            .MapAsync(rvs => rvs.Zip(typeFields.Select(tf => tf.Item1)))
            .MapAsync(rvt => rvt.Select(r =>
                Try(() => _serDes.Deserialize(r.First!, r.Second))
                    .OnFail(_ => Option<object>.None())).ToArray());

    public Either<Error, Unit> Set<T>(string key, string field, T value) =>
        Try(() => _database.HashSet(key, field, _serDes.Serialize(value)))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Map(_ => Unit.Default);

    public Either<Error, Unit> Set<T>(string key, params (string, T)[] pairs) =>
        Try(() =>
            Unit.Default.Tee(_ =>
                _database.HashSet(key, pairs.Select(t => new HashEntry(t.Item1, _serDes.Serialize(t.Item2))).ToArray())))
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Task<Either<Error, Unit>> SetAsync<T>(string key, string field, T value) =>
        TryAsync(() =>
            _database
                .HashSetAsync(key, field, _serDes.Serialize(value)))
            .ToEither().MapLeftAsync(e => Error.New(e))
            .MapAsync(_ => Unit.Default);

    public Task<Either<Error, Unit>> SetAsync<T>(string key, params (string, T)[] pairs) =>
        TryAsync(() => _database
                .HashSetAsync(key, pairs.Select(t => new HashEntry(t.Item1, _serDes.Serialize(t.Item2))).ToArray())
                .ToTaskUnit<object>())
            .ToEither().MapLeftAsync(e => Error.New(e));

    public Either<Error, Option<T[]>> GetValues<T>(string key) =>
        Try(() => _database.HashValues(key))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Bind(vs => Try(() => _serDes.Deserialize<T>(vs)).ToEither()
                    .MapLeft(e => Error.New(e)));

    public Task<Either<Error, Option<T[]>>> GetValuesAsync<T>(string key) =>
        TryAsync(() => _database.HashValuesAsync(key))
            .ToEither().MapLeftAsync(e => Error.New(e))
            .BindAsync(vs =>
                Try(() => _serDes.Deserialize<T>(vs)).ToEither().MapLeft(e => Error.New(e)));

    public Either<Error, Option<(string, T)[]>> GetAll<T>(string key) =>
        Try(() => _database.HashGetAll(key))
            .ToEither().MapLeft(e => Error.New(e))
            .Bind(entries =>
                Try(() => _serDes.Deserialize<T>(entries))
                    .ToEither()
                    .MapLeft(e => Error.New(e)));

    public Task<Either<Error, Option<(string, T)[]>>> GetAllAsync<T>(string key) =>
        TryAsync(() => _database.HashGetAllAsync(key))
            .ToEither().MapLeftAsync(e => Error.New(e))
            .BindAsync(entries =>
                Try(() => _serDes.Deserialize<T>(entries)).ToEither().MapLeft(e => Error.New(e)));

    public Either<Error, Option<string[]>> GetFieldKeys(string key) =>
        Try(() => _database.HashKeys(key))
            .ToEither().MapLeft(e => Error.New(e))
            .Map(v => v.ToOption(vv => vv.Length == 0))
            .Bind(v =>
                Try(() => v
                    .Map(i => i
                    .Select(_ => _.ToString())
                    .Select(_ => _!).ToArray()))
                .ToEither()
                .MapLeft(e => Error.New(e))
            );

    public Task<Either<Error, Option<string[]>>> GetFieldKeysAsync(string key) =>
        TryAsync(() => _database.HashKeysAsync(key))
            .ToEither().MapLeftAsync(e => Error.New(e))
            .MapAsync(v => v.ToOption(vv => vv.Length == 0))
            .BindAsync(v =>
                Try(() => v
                        .Map(i => i
                            .Select(_ => _.ToString())
                            .Select(_ => _!).ToArray()))
                    .ToEither().MapLeft(e => Error.New(e)));
}