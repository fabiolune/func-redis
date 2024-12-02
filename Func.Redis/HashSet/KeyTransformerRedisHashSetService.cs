namespace Func.Redis.HashSet;

/// <exclude />
internal class KeyTransformerRedisHashSetService(IRedisHashSetService service, Func<string, string> keyMapper) : IRedisHashSetService
{
    private readonly Func<string, string> _keyMapper = keyMapper;
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
    public Task<Either<Error, Option<object>[]>> GetAsync(string key, params (Type, string)[] typeFields) => _service.GetAsync(_keyMapper(key), typeFields);
    public Either<Error, Option<string[]>> GetFieldKeys(string key) => _service.GetFieldKeys(_keyMapper(key));
    public Task<Either<Error, Option<string[]>>> GetFieldKeysAsync(string key) => _service.GetFieldKeysAsync(_keyMapper(key));
    public Either<Error, Option<T[]>> GetValues<T>(string key) => _service.GetValues<T>(_keyMapper(key));
    public Task<Either<Error, Option<T[]>>> GetValuesAsync<T>(string key) => _service.GetValuesAsync<T>(_keyMapper(key));
    public Either<Error, Unit> Set<T>(string key, string field, T value) => _service.Set(_keyMapper(key), field, value);
    public Either<Error, Unit> Set<T>(string key, params (string, T)[] pairs) => _service.Set(_keyMapper(key), pairs);
    public Task<Either<Error, Unit>> SetAsync<T>(string key, string field, T value) => _service.SetAsync(_keyMapper(key), field, value);
    public Task<Either<Error, Unit>> SetAsync<T>(string key, params (string, T)[] pairs) => _service.SetAsync(_keyMapper(key), pairs);
}
