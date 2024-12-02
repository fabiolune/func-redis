namespace Func.Redis.Key;

public interface IRedisKeyService
{
    /// <summary>
    /// Delete a key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Either<Error, Unit> Delete(string key);
    /// <summary>
    /// Delete multiple keys
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    Either<Error, Unit> Delete(params string[] keys);
    /// <inheritdoc cref="Delete(string)"/>
    Task<Either<Error, Unit>> DeleteAsync(string key);
    /// <inheritdoc cref="Delete(string[])"/>
    Task<Either<Error, Unit>> DeleteAsync(params string[] keys);
    /// <summary>
    /// Get the value stored at a key. When the key does not exist, return None
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Either<Error, Option<T>> Get<T>(string key);
    /// <summary>
    /// Get the values stored at multiple keys. When a key does not exist, return None
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="keys"></param>
    /// <returns></returns>
    Either<Error, Option<T>[]> Get<T>(params string[] keys);
    /// <inheritdoc cref="Get{T}(string)"/>
    Task<Either<Error, Option<T>>> GetAsync<T>(string key);
    /// <inheritdoc cref="Get{T}(string[])"/>
    Task<Either<Error, Option<T>[]>> GetAsync<T>(params string[] keys);
    /// <summary>
    /// Set a value at a key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Either<Error, Unit> Set<T>(string key, T value);
    /// <summary>
    /// Set values at multiple keys
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pairs"></param>
    /// <returns></returns>
    Either<Error, Unit> Set<T>(params (string, T)[] pairs);
    /// <inheritdoc cref="Set{T}(string, T)"/>
    Task<Either<Error, Unit>> SetAsync<T>(string key, T value);
    /// <inheritdoc cref="Set{T}(string, T[])"/>
    Task<Either<Error, Unit>> SetAsync<T>(params (string, T)[] pairs);
    /// <summary>
    /// Rename a key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newKey"></param>
    /// <returns></returns>
    Either<Error, Unit> RenameKey(string key, string newKey);
    /// <inheritdoc cref="RenameKey(string, string)"/>
    Task<Either<Error, Unit>> RenameKeyAsync(string key, string newKey);
    /// <summary>
    /// Get all keys matching a pattern
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    Either<Error, string[]> GetKeys(string pattern);
    /// <inheritdoc cref="GetKeys(string)"/>
    Task<Either<Error, string[]>> GetKeysAsync(string pattern);
}
