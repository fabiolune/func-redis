using TinyFp;

namespace Func.Redis;

public interface IRedisHashSetService
{
    Either<Error, Unit> Delete(string key, string field);
    Either<Error, Unit> Delete(string key, params string[] fields);
    Task<Either<Error, Unit>> DeleteAsync(string key, string field);
    Task<Either<Error, Unit>> DeleteAsync(string key, params string[] fields);
    Either<Error, Option<T>> Get<T>(string key, string field);
    Either<Error, Option<T>[]> Get<T>(string key, params string[] fields);
    Either<Error, Option<object>[]> Get(string key, params (Type, string)[] typeFields);
    Task<Either<Error, Option<T>>> GetAsync<T>(string key, string field);
    Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, params string[] fields);
    Task<Either<Error, Option<object>[]>> GetAsync(string key, (Type, string)[] typeFields);
    Either<Error, Unit> Set<T>(string key, string field, T value);
    Either<Error, Unit> Set<T>(string key, params (string, T)[] pairs);
    Task<Either<Error, Unit>> SetAsync<T>(string key, string field, T value);
    Task<Either<Error, Unit>> SetAsync<T>(string key, params (string, T)[] pairs);
    Either<Error, Option<T[]>> GetValues<T>(string key);
    Task<Either<Error, Option<T[]>>> GetValuesAsync<T>(string key);
    Either<Error, Option<(string, T)[]>> GetAll<T>(string key);
    Task<Either<Error, Option<(string, T)[]>>> GetAllAsync<T>(string key);
    Either<Error, Option<string[]>> GetFieldKeys(string key);
    Task<Either<Error, Option<string[]>>> GetFieldKeysAsync(string key);
}