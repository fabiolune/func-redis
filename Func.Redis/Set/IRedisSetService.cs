namespace Func.Redis.Set;
public interface IRedisSetService
{
    /// <summary>
    /// Add a value to a set key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Either<Error, Unit> Add<T>(string key, T value);
    /// <inheritdoc cref="Add{T}(string, T)"/>
    Task<Either<Error, Unit>> AddAsync<T>(string key, T value);
    /// <summary>
    /// Delete a value from a set key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Either<Error, Unit> Delete<T>(string key, T value);
    /// <summary>
    /// Delete multiple values from a set key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    Either<Error, Unit> Delete<T>(string key, params T[] values);
    /// <inheritdoc cref="Delete{T}(string, T)"/>
    Task<Either<Error, Unit>> DeleteAsync<T>(string key, T value);
    /// <inheritdoc cref="Delete{T}(string, T[])"/>
    Task<Either<Error, Unit>> DeleteAsync<T>(string key, params T[] values);
    /// <summary>
    /// Get the difference between two set keys
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <returns></returns>
    Either<Error, T[]> Difference<T>(string key1, string key2);
    /// <inheritdoc cref="Difference{T}(string, string)"/>
    Task<Either<Error, T[]>> DifferenceAsync<T>(string key1, string key2);
    /// <summary>
    /// Get all values from a set key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Either<Error, Option<T>[]> GetAll<T>(string key);
    /// <inheritdoc cref="GetAll{T}(string)"/>
    Task<Either<Error, Option<T>[]>> GetAllAsync<T>(string key);
    /// <summary>
    /// Get the intersection between two set keys
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <returns></returns>
    Either<Error, T[]> Intersect<T>(string key1, string key2);
    /// <inheritdoc cref="Intersect{T}(string, string)"/>
    Task<Either<Error, T[]>> IntersectAsync<T>(string key1, string key2);
    /// <summary>
    /// Removes and returns an item from a set key. When the key does not exist, return none
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Either<Error, Option<T>> Pop<T>(string key);
    /// <inheritdoc cref="Pop{T}(string)"/>
    Task<Either<Error, Option<T>>> PopAsync<T>(string key);
    /// <summary>
    /// Get the size of a set key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Either<Error, long> Size(string key);
    /// <inheritdoc cref="Size(string)"/>
    Task<Either<Error, long>> SizeAsync(string key);
    /// <summary>
    /// Get the union between two set keys
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <returns></returns>
    Either<Error, T[]> Union<T>(string key1, string key2);
    /// <inheritdoc cref="Union{T}(string, string)"/>
    Task<Either<Error, T[]>> UnionAsync<T>(string key1, string key2);
}