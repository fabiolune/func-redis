namespace Func.Redis.Set;
public interface IRedisSetService
{
    /// <summary>
    /// Adds a value to the collection with the specified key.
    /// </summary>
    /// <remarks>The method ensures that the key-value pair is added to the collection. If the key already
    /// exists,  the operation may fail depending on the implementation, and an <see cref="Error"/> will be
    /// returned.</remarks>
    /// <typeparam name="T">The type of the value to add.</typeparam>
    /// <param name="key">The key associated with the value. Cannot be null or empty.</param>
    /// <param name="value">The value to add. Cannot be null.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see langword="Unit"/> if
    /// the addition is successful, or an <see cref="Error"/>  if the operation fails.</returns>
    Either<Error, Unit> Add<T>(string key, T value);

    /// <inheritdoc cref="Add{T}(string, T)"/>
    Task<Either<Error, Unit>> AddAsync<T>(string key, T value);

    /// <summary>
    /// Deletes an item identified by the specified key and value from the underlying data store.
    /// </summary>
    /// <remarks>The method performs a deletion operation and returns a result encapsulated in an <see
    /// cref="Either{Error, Unit}"/>.  If the operation fails, the <see cref="Error"/> provides details about the
    /// failure, such as invalid input or a missing item.</remarks>
    /// <typeparam name="T">The type of the value associated with the key.</typeparam>
    /// <param name="key">The unique key identifying the item to delete. Cannot be null or empty.</param>
    /// <param name="value">The value associated with the key, used for validation or additional context. Cannot be null.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see langword="Unit"/> if
    /// the deletion is successful, or an <see cref="Error"/> if the operation fails.</returns>
    Either<Error, Unit> Delete<T>(string key, T value);

    /// <summary>
    /// Deletes the specified values associated with the given key.
    /// </summary>
    /// <remarks>This method does not guarantee the existence of the specified values prior to deletion. 
    /// Ensure the key and values are valid and consistent with the underlying data store.</remarks>
    /// <typeparam name="T">The type of the values to delete.</typeparam>
    /// <param name="key">The key identifying the values to delete. Cannot be null or empty.</param>
    /// <param name="values">The values to delete. If no values are provided, the method performs no operation.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see langword="true"/> if
    /// the deletion is successful; otherwise, returns an <see cref="Error"/> describing the failure.</returns>
    Either<Error, Unit> Delete<T>(string key, params T[] values);

    /// <inheritdoc cref="Delete{T}(string, T)"/>
    Task<Either<Error, Unit>> DeleteAsync<T>(string key, T value);

    /// <inheritdoc cref="Delete{T}(string, T[])"/>
    Task<Either<Error, Unit>> DeleteAsync<T>(string key, params T[] values);

    /// <summary>
    /// Computes the difference between two sets of data identified by the specified keys.
    /// </summary>
    /// <remarks>The difference is calculated as the elements present in the first set but not in the second
    /// set. If either key does not correspond to a valid set, an error is returned.</remarks>
    /// <typeparam name="T">The type of elements in the sets.</typeparam>
    /// <param name="key1">The key identifying the first set of data. Cannot be null or empty.</param>
    /// <param name="key2">The key identifying the second set of data. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{Error, T[]}"/> containing either an error if the operation fails,  or an array of elements
    /// representing the difference between the two sets.</returns>
    Either<Error, T[]> Difference<T>(string key1, string key2);

    /// <inheritdoc cref="Difference{T}(string, string)"/>
    Task<Either<Error, T[]>> DifferenceAsync<T>(string key1, string key2);
    /// <summary>
    /// Retrieves all values associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the values to retrieve.</typeparam>
    /// <param name="key">The key used to look up the values. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{Error, Option{T}[]}"/> containing either an error or an array of optional values. If the
    /// key is not found, the array will be empty.</returns>
    Either<Error, Option<T>[]> GetAll<T>(string key);

    /// <inheritdoc cref="GetAll{T}(string)"/>
    Task<Either<Error, Option<T>[]>> GetAllAsync<T>(string key);

    /// <summary>
    /// Computes the intersection of two sets stored under the specified keys.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sets.</typeparam>
    /// <param name="key1">The key identifying the first set. Cannot be null or empty.</param>
    /// <param name="key2">The key identifying the second set. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{Error, T[]}"/> containing either an error if the operation fails,  or an array of elements
    /// representing the intersection of the two sets. If either key does not exist or the sets are empty, the result
    /// will be an empty array.</returns>
    Either<Error, T[]> Intersect<T>(string key1, string key2);

    /// <inheritdoc cref="Intersect{T}(string, string)"/>
    Task<Either<Error, T[]>> IntersectAsync<T>(string key1, string key2);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Either<Error, Option<T>> Pop<T>(string key);

    /// <inheritdoc cref="Pop{T}(string)"/>
    Task<Either<Error, Option<T>>> PopAsync<T>(string key);

    /// <summary>
    /// Retrieves the size, in bytes, of the object associated with the specified key.
    /// </summary>
    /// <param name="key">The unique identifier of the object whose size is to be retrieved. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/> if the operation fails,  or a
    /// <see langword="long"/> representing the size of the object in bytes if successful.</returns>
    Either<Error, long> Size(string key);

    /// <inheritdoc cref="Size(string)"/>
    Task<Either<Error, long>> SizeAsync(string key);

    /// <summary>
    /// Combines the values associated with two specified keys into a single array.
    /// </summary>
    /// <remarks>If either key does not exist or an error occurs during the operation, the result will contain
    /// an <see cref="Error"/> describing the issue. Otherwise, the result will contain the combined  values from both
    /// keys, with duplicates removed if applicable.</remarks>
    /// <typeparam name="T">The type of the values associated with the keys.</typeparam>
    /// <param name="key1">The first key whose associated values will be included in the union.</param>
    /// <param name="key2">The second key whose associated values will be included in the union.</param>
    /// <returns>An <see cref="Either{Error, T[]}"/> containing either an error if the operation fails,  or an array of values of
    /// type <typeparamref name="T"/> representing the union of the values  associated with <paramref name="key1"/> and
    /// <paramref name="key2"/>.</returns>
    Either<Error, T[]> Union<T>(string key1, string key2);

    /// <inheritdoc cref="Union{T}(string, string)"/>
    Task<Either<Error, T[]>> UnionAsync<T>(string key1, string key2);
}