namespace Func.Redis.HashSet;

public interface IRedisHashSetService
{
    /// <summary>
    /// Delete a field from a key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    Either<Error, Unit> Delete(string key, string field);
    /// <summary>
    /// Delete multiple fields from a key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    Either<Error, Unit> Delete(string key, params string[] fields);
    /// <inheritdoc cref="Delete(string, string)"/>
    Task<Either<Error, Unit>> DeleteAsync(string key, string field);
    /// <inheritdoc cref="Delete(string, string[])"/>
    Task<Either<Error, Unit>> DeleteAsync(string key, params string[] fields);
    /// <summary>
    /// Get the value stored at a field of a key. When the key or field does not exist, return None
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    Either<Error, Option<T>> Get<T>(string key, string field);
    /// <summary>
    /// Get the values stored at multiple fields of a key. When the key or field does not exist, return None
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    Either<Error, Option<T>[]> Get<T>(string key, params string[] fields);
    /// <summary>
    /// Get the values stored at multiple fields of a key. When the key or field does not exist, return None
    /// </summary>
    /// <param name="key"></param>
    /// <param name="typeFields"></param>
    /// <returns></returns>
    Either<Error, Option<object>[]> Get(string key, params (Type, string)[] typeFields);
    /// <inheritdoc cref="Get{T}(string, string)"/>/>
    Task<Either<Error, Option<T>>> GetAsync<T>(string key, string field);
    /// <inheritdoc cref="Get{T}(string, string[])"/>
    Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, params string[] fields);
    /// <inheritdoc cref="GetAsync(string, ValueTuple{Type, string}[])"/>
    Task<Either<Error, Option<object>[]>> GetAsync(string key, params (Type, string)[] typeFields);
    /// <summary>
    /// Set a value at a field of a key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Either<Error, Unit> Set<T>(string key, string field, T value);
    /// <summary>
    /// Set values at multiple fields of a key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="pairs"></param>
    /// <returns></returns>
    Either<Error, Unit> Set<T>(string key, params (string, T)[] pairs);
    /// <inheritdoc cref="Set{T}(string, string, T)"/>
    Task<Either<Error, Unit>> SetAsync<T>(string key, string field, T value);
    /// <inheritdoc cref="Set{T}(string, ValueTuple{string, T}[])"/>
    Task<Either<Error, Unit>> SetAsync<T>(string key, params (string, T)[] pairs);
    /// <summary>
    /// Get the values stored at all fields of a key. When the key does not exist, return None
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Either<Error, Option<T[]>> GetValues<T>(string key);
    /// <inheritdoc cref="GetValues{T}(string)"/>
    Task<Either<Error, Option<T[]>>> GetValuesAsync<T>(string key);
    /// <summary>
    /// Get all the fields and values stored at a key. When the key does not exist, return None
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Either<Error, Option<(string, T)[]>> GetAll<T>(string key);
    /// <inheritdoc cref="GetAll{T}(string)"/>
    Task<Either<Error, Option<(string, T)[]>>> GetAllAsync<T>(string key);
    /// <summary>
    /// Get all the fields stored at a key. When the key does not exist, return None
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Either<Error, Option<string[]>> GetFieldKeys(string key);
    /// <inheritdoc cref="GetFieldKeys(string)"/>
    Task<Either<Error, Option<string[]>>> GetFieldKeysAsync(string key);
}