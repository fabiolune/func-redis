
namespace Func.Redis.List;

public interface IRedisListService
{
    /// <summary>
    /// Add an item at the end of a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Either<Error, Unit> Append<T>(string key, T value);
    /// <summary>
    /// Add multiple items at the end of a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    Either<Error, Unit> Append<T>(string key, params T[] values);
    /// <inheritdoc cref="Append{T}(string, T)" /> >
    Task<Either<Error, Unit>> AppendAsync<T>(string key, T value);
    /// <inheritdoc cref="Append{T}(string, T[])"/>
    Task<Either<Error, Unit>> AppendAsync<T>(string key, params T[] values);
    /// <summary>
    /// Get the item at a specific index. When the item does not exist, return None
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    Either<Error, Option<T>> Get<T>(string key, long index);
    /// <summary>
    /// Get the items in an index interval. When the index have no data, return none
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="start"></param>
    /// <param name="stop"></param>
    /// <returns></returns>
    Either<Error, Option<T>[]> Get<T>(string key, long start, long stop);
    /// <inheritdoc cref="Get{T}(string, long)"/>
    Task<Either<Error, Option<T>>> GetAsync<T>(string key, long index);
    /// <inheritdoc cref="Get{T}(string, long, long)"/>
    Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, long start, long stop);
    /// <summary>
    /// Removes and returns an item from the end of a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Either<Error, Option<T>> Pop<T>(string key);
    /// <summary>
    /// Removes multiple items from the end of a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Either<Error, Option<T>[]> Pop<T>(string key, long count);
    ///  <inheritdoc cref="Pop{T}(string)"/>
    Task<Either<Error, Option<T>>> PopAsync<T>(string key);
    /// <inheritdoc cref="Pop{T}(string, long)"/>
    Task<Either<Error, Option<T>[]>> PopAsync<T>(string key, long count);
    /// <summary>
    /// Add an item at the beginning of a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    Either<Error, Unit> Prepend<T>(string key, params T[] values);
    /// <summary>
    ///  Add multiple items at the beginning of a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Either<Error, Unit> Prepend<T>(string key, T value);
    /// <inheritdoc cref="Prepend{T}(string, T)"/>
    Task<Either<Error, Unit>> PrependAsync<T>(string key, params T[] values);
    /// <inheritdoc cref="Prepend{T}(string, T[])"/>
    Task<Either<Error, Unit>> PrependAsync<T>(string key, T value);
    /// <summary>
    /// Removes an item from the beginning of a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Either<Error, Option<T>> Shift<T>(string key);
    /// <summary>
    /// Removes multiple items from the beginning of a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Either<Error, Option<T>[]> Shift<T>(string key, long count);
    /// <inheritdoc cref="Shift{T}(string)"/>
    Task<Either<Error, Option<T>>> ShiftAsync<T>(string key);
    /// <inheritdoc cref="Shift{T}(string, long)"/>
    Task<Either<Error, Option<T>[]>> ShiftAsync<T>(string key, long count);
    /// <summary>
    /// Get the size of a list
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Either<Error, long> Size(string key);
    /// <inheritdoc cref="Size(string)"/>
    Task<Either<Error, long>> SizeAsync(string key);
}