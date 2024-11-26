using Func.Redis.SerDes;
using TinyFp.Extensions;

namespace Func.Redis.Key;

public class RedisKeyService(
    ISourcesProvider sourcesProvider,
    IRedisSerDes serDes) : IRedisKeyService
{
    private static readonly Error SetError = Error.New("Redis KEY SET Error");
    private static readonly Error RenameError = Error.New("Error renaming key");

    private readonly IDatabase _database = sourcesProvider.GetDatabase();
    private readonly ISourcesProvider _sourcesProvider = sourcesProvider;
    private readonly IRedisSerDes _serDes = serDes;

    public Either<Error, Unit> Delete(string key) =>
        Try(() => _database.KeyDelete(key).Map(_ => Unit.Default))
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Either<Error, Unit> Delete(params string[] keys) =>
        Try(() => _database.KeyDelete(ConvertToKeys(keys)).Map(_ => Unit.Default))
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Task<Either<Error, Unit>> DeleteAsync(string key) =>
        TryAsync(() => _database.KeyDeleteAsync(key))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .MapAsync(_ => Unit.Default);

    public Task<Either<Error, Unit>> DeleteAsync(params string[] keys) =>
        TryAsync(() => _database.KeyDeleteAsync(ConvertToKeys(keys)))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .MapAsync(_ => Unit.Default);

    public Either<Error, Option<T>> Get<T>(string key) =>
        Try(() => _database.StringGet(key).ToOption(r => r.IsNullOrEmpty).Bind(_serDes.Deserialize<T>))
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Either<Error, Option<T>[]> Get<T>(params string[] keys) =>
        Try(() => _database.StringGet(ConvertToKeys(keys)).Select(_serDes.Deserialize<T>).ToArray())
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Task<Either<Error, Option<T>>> GetAsync<T>(string key) =>
        TryAsync(() => _database.StringGetAsync(key))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(res => Try(() => _serDes.Deserialize<T>(res)).ToEither().MapLeft(e => Error.New(e)));

    public Task<Either<Error, Option<T>[]>> GetAsync<T>(params string[] keys) =>
        TryAsync(() => _database.StringGetAsync(ConvertToKeys(keys)))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(res => Try(() => res.Select(_serDes.Deserialize<T>).ToArray()).ToEither().MapLeft(e => Error.New(e)));

    public Either<Error, string[]> GetKeys(string pattern) =>
        Try(() => _sourcesProvider
                .GetServers()
                .Select(s => s.Keys(pattern: pattern))
                .SelectMany(k => k)
                .Select(k => k.ToString())
                .ToArray())
            .ToEither()
            .MapLeft(e => Error.New(e));

    public Task<Either<Error, string[]>> GetKeysAsync(string pattern) =>
        TryAsync(() => _sourcesProvider
                .GetServers()
                .Select(s => s.KeysAsync(pattern: pattern))
                .Merge()
                .Select(k => k.ToString())
                .ToArrayAsync()
                .AsTask())
        .ToEither()
        .MapLeftAsync(e => Error.New(e));

    public Either<Error, Unit> RenameKey(string key, string newKey) =>
        Try(() => _database.KeyRename(key, newKey))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Bind(res => res.ToEither(_ => Unit.Default, b => !b, RenameError));

    public Task<Either<Error, Unit>> RenameKeyAsync(string key, string newKey) =>
        TryAsync(() => _database.KeyRenameAsync(key, newKey))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(res => res.ToEither(_ => Unit.Default, b => !b, RenameError));

    public Either<Error, Unit> Set<T>(string key, T value) =>
        Try(() => _database.StringSet(key, _serDes.Serialize(value)))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Bind(res => res.ToEither(_ => Unit.Default, b => !b, SetError));

    public Either<Error, Unit> Set<T>(params (string, T)[] pairs) =>
        Try(() => _database.StringSet(ConvertToKeyValues(pairs)))
            .ToEither()
            .MapLeft(e => Error.New(e))
            .Bind(res => res.ToEither(_ => Unit.Default, b => !b, SetError));

    public Task<Either<Error, Unit>> SetAsync<T>(string key, T value) =>
        TryAsync(() => _database.StringSetAsync(key, _serDes.Serialize(value)))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(res => res.ToEither(_ => Unit.Default, b => !b, SetError));

    public Task<Either<Error, Unit>> SetAsync<T>(params (string, T)[] pairs) =>
        TryAsync(() => _database.StringSetAsync(ConvertToKeyValues(pairs)))
            .ToEither()
            .MapLeftAsync(e => Error.New(e))
            .BindAsync(res => res.ToEither(_ => Unit.Default, b => !b, SetError));

    private static RedisKey[] ConvertToKeys(string[] keys) =>
        keys
            .Select(k => new RedisKey(k))
            .ToArray();

    private KeyValuePair<RedisKey, RedisValue>[] ConvertToKeyValues<T>(params (string, T)[] pairs) =>
        pairs
            .Select(kv => KeyValuePair.Create(new RedisKey(kv.Item1), _serDes.Serialize(kv.Item2)))
            .ToArray();
}