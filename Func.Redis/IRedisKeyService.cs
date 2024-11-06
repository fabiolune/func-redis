using TinyFp;

namespace Func.Redis;

public interface IRedisKeyService
{
    Either<Error, Unit> Delete(string key);
    Either<Error, Unit> Delete(params string[] keys);
    Task<Either<Error, Unit>> DeleteAsync(string key);
    Task<Either<Error, Unit>> DeleteAsync(params string[] keys);
    Either<Error, Option<T>> Get<T>(string key);
    Either<Error, Option<T>[]> Get<T>(params string[] keys);
    Task<Either<Error, Option<T>>> GetAsync<T>(string key);
    Task<Either<Error, Option<T>[]>> GetAsync<T>(params string[] keys);
    Either<Error, Unit> Set<T>(string key, T value);
    Either<Error, Unit> Set<T>(params (string, T)[] pairs);
    Task<Either<Error, Unit>> SetAsync<T>(string key, T value);
    Task<Either<Error, Unit>> SetAsync<T>(params (string, T)[] pairs);
    Either<Error, Unit> RenameKey(string key, string newKey);
    Task<Either<Error, Unit>> RenameKeyAsync(string key, string newKey);
    Either<Error, string[]> GetKeys(string pattern);
    Task<Either<Error, string[]>> GetKeysAsync(string pattern);
}
