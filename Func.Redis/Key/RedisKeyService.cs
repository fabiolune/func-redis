using Func.Redis.SerDes;
using Func.Redis.Utils;
using TinyFp.Extensions;
using static Func.Redis.Utils.FunctionUtilities;

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
        Wrap(() => _database.KeyDelete(key), FunctionUtilities<bool>.ToUnit);

    public Either<Error, Unit> Delete(params string[] keys) =>
        Wrap(() => _database.KeyDelete(ConvertToKeys(keys)), FunctionUtilities<long>.ToUnit);

    public Task<Either<Error, Unit>> DeleteAsync(string key) =>
        WrapAsync(() => _database.KeyDeleteAsync(key), FunctionUtilities<bool>.ToUnit);

    public Task<Either<Error, Unit>> DeleteAsync(params string[] keys) =>
        WrapAsync(() => _database.KeyDeleteAsync(ConvertToKeys(keys)), FunctionUtilities<long>.ToUnit);

    public Either<Error, Option<T>> Get<T>(string key) =>
        Wrap(() => _database.StringGet(key).Map(_serDes.Deserialize<T>));

    public Either<Error, Option<T>[]> Get<T>(params string[] keys) =>
        Wrap(() => _database.StringGet(ConvertToKeys(keys)), res => res.Select(_serDes.Deserialize<T>).ToArray());

    public Task<Either<Error, Option<T>>> GetAsync<T>(string key) =>
        WrapUnsafeAsync(() => _database.StringGetAsync(key), _serDes.Deserialize<T>);

    public Task<Either<Error, Option<T>[]>> GetAsync<T>(params string[] keys) =>
        WrapUnsafeAsync(() => _database.StringGetAsync(ConvertToKeys(keys)), res => res.Select(_serDes.Deserialize<T>).ToArray());

    public Either<Error, string[]> GetKeys(string pattern) =>
        Wrap(() => _sourcesProvider
            .GetServers()
            .Select(s => s.Keys(pattern: pattern))
            .SelectMany(k => k)
            .Select(k => k.ToString())
            .ToArray());

    public Task<Either<Error, string[]>> GetKeysAsync(string pattern) =>
        WrapAsync(() => _sourcesProvider
            .GetServers()
            .Select(s => s.KeysAsync(pattern: pattern))
            .Merge()
            .Select(k => k.ToString())
            .ToArrayAsync()
            .AsTask());

    public Either<Error, Unit> RenameKey(string key, string newKey) =>
        Wrap(() => _database.KeyRename(key, newKey), RenameError);

    public Task<Either<Error, Unit>> RenameKeyAsync(string key, string newKey) =>
        WrapAsync(() => _database.KeyRenameAsync(key, newKey), RenameError);

    public Either<Error, Unit> Set<T>(string key, T value) =>
        Wrap(() => _database.StringSet(key, _serDes.Serialize(value)), SetError);

    public Either<Error, Unit> Set<T>(params (string, T)[] pairs) =>
        Wrap(() => _database.StringSet(ConvertToKeyValues(pairs)), SetError);

    public Task<Either<Error, Unit>> SetAsync<T>(string key, T value) =>
        WrapAsync(() => _database.StringSetAsync(key, _serDes.Serialize(value)), SetError);

    public Task<Either<Error, Unit>> SetAsync<T>(params (string, T)[] pairs) =>
        WrapAsync(() => _database.StringSetAsync(ConvertToKeyValues(pairs)), SetError);

    private static RedisKey[] ConvertToKeys(string[] keys) =>
        keys
            .Select(k => new RedisKey(k))
            .ToArray();

    private KeyValuePair<RedisKey, RedisValue>[] ConvertToKeyValues<T>(params (string, T)[] pairs) =>
        pairs
            .Select(kv => KeyValuePair.Create(new RedisKey(kv.Item1), _serDes.Serialize(kv.Item2)))
            .ToArray();
}