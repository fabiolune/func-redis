namespace Func.Redis.Key;

public interface IRedisKeyService
{
    /// <summary>
    /// Deletes the item associated with the specified key.
    /// </summary>
    /// <param name="key">The key identifying the item to delete. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see cref="Unit"/> if the
    /// deletion is successful, or an <see cref="Error"/> if the operation fails.</returns>
    Either<Error, Unit> Delete(string key);

    /// <summary>
    /// Deletes the specified items identified by their keys.
    /// </summary>
    /// <remarks>This method attempts to delete all items corresponding to the provided keys. If one or more
    /// keys cannot be deleted, the operation will fail and return an <see cref="Error"/>.</remarks>
    /// <param name="keys">An array of keys representing the items to delete. Each key must be a non-null, non-empty string.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation. If the operation succeeds, the
    /// result will contain <see langword="Unit"/>. If the operation fails, the result will contain an <see
    /// cref="Error"/> describing the failure.</returns>
    Either<Error, Unit> Delete(string[] keys);

    /// <inheritdoc cref="Delete(string)"/>
    Task<Either<Error, Unit>> DeleteAsync(string key);

    /// <inheritdoc cref="Delete(string[])"/>
    Task<Either<Error, Unit>> DeleteAsync(string[] keys);

    /// <summary>
    /// Retrieves the value associated with the specified key, if it exists.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key used to locate the value. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{Error, Option{T}}"/> containing either an error if the operation fails,  or an <see
    /// cref="Option{T}"/> representing the value if found. Returns <see langword="None"/>  if the key does not exist.</returns>
    Either<Error, Option<T>> Get<T>(string key);

    /// <summary>
    /// Retrieves an array of optional values associated with the specified keys.
    /// </summary>
    /// <typeparam name="T">The type of the values to retrieve.</typeparam>
    /// <param name="keys">The keys used to locate the values. Each key must be a non-null, non-empty string.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/> if the operation fails, or an
    /// array of <see cref="Option{T}"/> representing the values associated with the specified keys. If a key does not
    /// have an associated value, the corresponding element in the array will be <see cref="Option{T}.None"/>.</returns>
    Either<Error, Option<T>[]> Get<T>(string[] keys);

    /// <inheritdoc cref="Get{T}(string)"/>
    Task<Either<Error, Option<T>>> GetAsync<T>(string key);

    /// <inheritdoc cref="Get{T}(string[])"/>
    Task<Either<Error, Option<T>[]>> GetAsync<T>(string[] keys);

    /// <summary>
    /// Stores a value in the underlying data store associated with the specified key.
    /// </summary>
    /// <remarks>The method may fail if the key is invalid, the value cannot be serialized, or if there is an
    /// issue with the data store.</remarks>
    /// <typeparam name="T">The type of the value to store.</typeparam>
    /// <param name="key">The key used to identify the stored value. Cannot be null or empty.</param>
    /// <param name="value">The value to store. Must be serializable and compatible with the data store.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see langword="Unit"/> if
    /// the operation succeeds, or an <see cref="Error"/> if it fails.</returns>
    Either<Error, Unit> Set<T>(string key, T value);

    /// <summary>
    /// Sets multiple key-value pairs in the underlying storage.
    /// </summary>
    /// <remarks>This method allows setting multiple key-value pairs in a single operation.  If any pair fails
    /// to be set, the operation will return an <see cref="Error"/> describing the issue.</remarks>
    /// <typeparam name="T">The type of the values to be stored.</typeparam>
    /// <param name="pairs">An array of tuples, where each tuple contains a key as a string and a value of type <typeparamref name="T"/>.
    /// The key must not be null or empty.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see langword="Unit"/> if
    /// the operation succeeds, or an <see cref="Error"/> if it fails.</returns>
    Either<Error, Unit> Set<T>(params (string, T)[] pairs);

    /// <inheritdoc cref="Set{T}(string, T)"/>
    Task<Either<Error, Unit>> SetAsync<T>(string key, T value);

    /// <inheritdoc cref="Set{T}(string, T[])"/>
    Task<Either<Error, Unit>> SetAsync<T>(params (string, T)[] pairs);

    /// <summary>
    /// Renames an existing key to a new key within the system.
    /// </summary>
    /// <remarks>If the operation succeeds, the key will be updated to the specified new key.  If the
    /// operation fails, an error will be returned indicating the reason for failure.</remarks>
    /// <param name="key">The current name of the key to be renamed. Cannot be null or empty.</param>
    /// <param name="newKey">The new name for the key. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> containing either an error describing the failure  or a unit value
    /// indicating success.</returns>
    Either<Error, Unit> RenameKey(string key, string newKey);

    /// <inheritdoc cref="RenameKey(string, string)"/>
    Task<Either<Error, Unit>> RenameKeyAsync(string key, string newKey);

    /// <summary>
    /// Retrieves an array of keys that match the specified pattern.
    /// </summary>
    /// <remarks>The method uses pattern matching to filter keys. Ensure the pattern syntax is valid for the
    /// underlying implementation. The operation may fail due to  connectivity issues or invalid patterns, in which case
    /// an <see cref="Error"/>  will be returned.</remarks>
    /// <param name="pattern">The pattern to match keys against. This can include wildcard characters  or other pattern-matching syntax
    /// supported by the underlying implementation.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/>  if the operation fails, or an
    /// array of strings representing the matching keys. If no keys match the pattern, the array will be empty.</returns>
    Either<Error, string[]> GetKeys(string pattern);

    /// <inheritdoc cref="GetKeys(string)"/>
    Task<Either<Error, string[]>> GetKeysAsync(string pattern);

}
