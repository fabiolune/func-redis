using Func.Redis.Extensions;
using Func.Redis.Models;
using TinyFp;
using TinyFp.Extensions;

namespace Func.Redis;

public class KeyTransformerRedisKeyService(IRedisKeyService service, Func<string, string> keyMapper, Func<string, string> inverseKeyMapper) : IRedisKeyService
{
    private readonly Func<string, string> _keyMapper = keyMapper;
    private readonly Func<string, string> _inverseKeyMapper = inverseKeyMapper;
    private readonly IRedisKeyService _service = service;

    public Either<Error, Unit> Delete(string key) => _service.Delete(_keyMapper(key));
    public Either<Error, Unit> Delete(params string[] keys) => _service.Delete(keys.Select(_keyMapper).ToArray());
    public Task<Either<Error, Unit>> DeleteAsync(string key) => _service.DeleteAsync(_keyMapper(key));
    public Task<Either<Error, Unit>> DeleteAsync(params string[] keys) => _service.DeleteAsync(keys.Select(_keyMapper).ToArray());
    public Either<Error, Option<T>> Get<T>(string key) => _service.Get<T>(_keyMapper(key));
    public Either<Error, Option<T>[]> Get<T>(params string[] keys) => _service.Get<T>(keys.Select(_keyMapper).ToArray());
    public Task<Either<Error, Option<T>>> GetAsync<T>(string key) => _service.GetAsync<T>(_keyMapper(key));
    public Task<Either<Error, Option<T>[]>> GetAsync<T>(params string[] keys) => _service.GetAsync<T>(keys.Select(_keyMapper).ToArray());
    public Either<Error, string[]> GetKeys(string pattern) => _service.GetKeys(_keyMapper(pattern)).Map(kk => kk.Select(_inverseKeyMapper).ToArray());
    public Task<Either<Error, string[]>> GetKeysAsync(string pattern) => _service.GetKeysAsync(_keyMapper(pattern)).MapAsync(kk => kk.Select(_inverseKeyMapper).ToArray());
    public Either<Error, Unit> RenameKey(string key, string newKey) => _service.RenameKey(_keyMapper(key), _keyMapper(newKey));
    public Task<Either<Error, Unit>> RenameKeyAsync(string key, string newKey) => _service.RenameKeyAsync(_keyMapper(key), _keyMapper(newKey));
    public Either<Error, Unit> Set<T>(string key, T value) => _service.Set(_keyMapper(key), value);
    public Either<Error, Unit> Set<T>(params (string, T)[] pairs) => _service.Set(pairs.Select(t => (_keyMapper(t.Item1), t.Item2)).ToArray());
    public Task<Either<Error, Unit>> SetAsync<T>(string key, T value) => _service.SetAsync(_keyMapper(key), value);
    public Task<Either<Error, Unit>> SetAsync<T>(params (string, T)[] pairs) => _service.SetAsync(pairs.Select(t => (_keyMapper(t.Item1), t.Item2)).ToArray());
}

public class KeyTransformerRedisHasSetService(IRedisHashSetService service, RedisKeyConfiguration config) : IRedisHashSetService
{
    private readonly Func<string, string> _keyMapper = config.GetKeyMapper();
    private readonly IRedisHashSetService _service = service;

    public Either<Error, Unit> Delete(string key, string field) => _service.Delete(_keyMapper(key), field);
    public Either<Error, Unit> Delete(string key, params string[] fields) => _service.Delete(_keyMapper(key), fields);
    public Task<Either<Error, Unit>> DeleteAsync(string key, string field) => _service.DeleteAsync(_keyMapper(key), field);
    public Task<Either<Error, Unit>> DeleteAsync(string key, params string[] fields) => _service.DeleteAsync(_keyMapper(key), fields);
    public Either<Error, Option<T>> Get<T>(string key, string field) => _service.Get<T>(_keyMapper(key), field);
    public Either<Error, Option<T>[]> Get<T>(string key, params string[] fields) => _service.Get<T>(_keyMapper(key), fields);
    public Either<Error, Option<object>[]> Get(string key, params (Type, string)[] typeFields) => _service.Get(_keyMapper(key), typeFields);
    public Either<Error, Option<(string, T)[]>> GetAll<T>(string key) => _service.GetAll<T>(_keyMapper(key));
    public Task<Either<Error, Option<(string, T)[]>>> GetAllAsync<T>(string key) => _service.GetAllAsync<T>(_keyMapper(key));
    public Task<Either<Error, Option<T>>> GetAsync<T>(string key, string field) => _service.GetAsync<T>(_keyMapper(key), field);
    public Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, params string[] fields) => _service.GetAsync<T>(_keyMapper(key), fields);
    public Task<Either<Error, Option<object>[]>> GetAsync(string key, (Type, string)[] typeFields) => _service.GetAsync(_keyMapper(key), typeFields);
    public Either<Error, Option<string[]>> GetFieldKeys(string key) => _service.GetFieldKeys(_keyMapper(key));
    public Task<Either<Error, Option<string[]>>> GetFieldKeysAsync(string key) => _service.GetFieldKeysAsync(_keyMapper(key));
    public Either<Error, Option<T[]>> GetValues<T>(string key) => _service.GetValues<T>(_keyMapper(key));
    public Task<Either<Error, Option<T[]>>> GetValuesAsync<T>(string key) => _service.GetValuesAsync<T>(_keyMapper(key));
    public Either<Error, Unit> Set<T>(string key, string field, T value) => _service.Set(_keyMapper(key), field, value);
    public Either<Error, Unit> Set<T>(string key, params (string, T)[] pairs) => _service.Set(_keyMapper(key), pairs);
    public Task<Either<Error, Unit>> SetAsync<T>(string key, string field, T value) => _service.SetAsync(_keyMapper(key), field, value);
    public Task<Either<Error, Unit>> SetAsync<T>(string key, params (string, T)[] pairs) => _service.SetAsync(_keyMapper(key), pairs);
}
