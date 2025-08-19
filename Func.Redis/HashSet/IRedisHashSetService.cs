namespace Func.Redis.HashSet;

public interface IRedisHashSetService
{
    /// <summary>
    /// Deletes a specific field associated with the given key from the data store.
    /// </summary>
    /// <param name="key">The key identifying the entity from which the field should be deleted. Cannot be null or empty.</param>
    /// <param name="field">The name of the field to delete. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation. Returns <see langword="Unit"/> if
    /// the deletion is successful, or an <see cref="Error"/> if the operation fails.</returns>
    Either<Error, Unit> Delete(string key, string field);

    /// <summary>
    /// Deletes specified fields associated with the given key from the data store.
    /// </summary>
    /// <remarks>This method performs a partial or complete deletion of fields associated with the specified
    /// key. If the key does not exist, the operation fails and returns an <see cref="Error"/>.</remarks>
    /// <param name="key">The unique identifier for the data entry to modify. Cannot be null or empty.</param>
    /// <param name="fields">An array of field names to delete. If no fields are specified, the method deletes all fields associated with the
    /// key.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation. Returns <see langword="Unit"/> if
    /// the operation succeeds, or an <see cref="Error"/> if the operation fails.</returns>
    Either<Error, Unit> Delete(string key, params string[] fields);

    /// <inheritdoc cref="Delete(string, string)"/>
    Task<Either<Error, Unit>> DeleteAsync(string key, string field);

    /// <inheritdoc cref="Delete(string, string[])"/>
    Task<Either<Error, Unit>> DeleteAsync(string key, params string[] fields);

    /// <summary>
    /// Retrieves the value associated with the specified key and field.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key identifying the collection or object to search. Cannot be null or empty.</param>
    /// <param name="field">The field within the collection or object to retrieve the value from. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{Error, Option{T}}"/> containing either an error or an optional value of type <typeparamref
    /// name="T"/>. If the key or field does not exist, the result will contain an empty <see cref="Option{T}"/>.</returns>
    Either<Error, Option<T>> Get<T>(string key, string field);

    /// <summary>
    /// Retrieves an array of optional values associated with the specified key and fields.
    /// </summary>
    /// <typeparam name="T">The type of the values to retrieve.</typeparam>
    /// <param name="key">The key used to identify the values. Cannot be null or empty.</param>
    /// <param name="fields">An optional array of field names to filter the values. If no fields are specified, all values associated with
    /// the key are retrieved.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/> if the operation fails,  or an
    /// array of <see cref="Option{T}"/> representing the retrieved values. The array will be empty if no values are
    /// found.</returns>
    Either<Error, Option<T>[]> Get<T>(string key, params string[] fields);

    /// <summary>
    /// Retrieves an array of optional objects based on the specified key and type-field pairs.
    /// </summary>
    /// <param name="key">The key used to identify the data to retrieve. Cannot be null or empty.</param>
    /// <param name="typeFields">A collection of type-field pairs, where each pair specifies the type and field name to query. The type
    /// represents the expected type of the data, and the field name identifies the specific field to retrieve.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/> object if the operation fails,
    /// or an array of <see cref="Option{T}"/> objects representing the retrieved data. Each element in the array
    /// corresponds to a type-field pair, and will be <see langword="null"/> if the data for that pair is unavailable.</returns>
    Either<Error, Option<object>[]> Get(string key, params (Type, string)[] typeFields);

    /// <inheritdoc cref="Get{T}(string, string)"/>/>
    Task<Either<Error, Option<T>>> GetAsync<T>(string key, string field);

    /// <inheritdoc cref="Get{T}(string, string[])"/>
    Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, params string[] fields);

    /// <inheritdoc cref="GetAsync(string, ValueTuple{Type, string}[])"/>
    Task<Either<Error, Option<object>[]>> GetAsync(string key, params (Type, string)[] typeFields);

    /// <summary>
    /// Sets the specified value for a given key and field in the data store.
    /// </summary>
    /// <remarks>This method updates the value for the specified field within the entry identified by the key.
    /// If the key or field does not exist, or if the operation fails, an <see cref="Error"/> is returned.</remarks>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    /// <param name="key">The key identifying the data store entry. Cannot be null or empty.</param>
    /// <param name="field">The field within the entry to set the value for. Cannot be null or empty.</param>
    /// <param name="value">The value to set for the specified field. Cannot be null.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see langword="Unit"/> if
    /// the operation succeeds, or an <see cref="Error"/> if it fails.</returns>
    Either<Error, Unit> Set<T>(string key, string field, T value);

    /// <summary>
    /// Sets multiple key-value pairs in the specified storage, associating each pair with the given key.
    /// </summary>
    /// <typeparam name="T">The type of the values to be stored.</typeparam>
    /// <param name="key">The key used to group the pairs in the storage. Cannot be null or empty.</param>
    /// <param name="pairs">An array of tuples, where each tuple contains a string identifier and a value of type <typeparamref name="T"/>.
    /// The identifiers must be unique within the specified key.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see langword="Unit"/> if
    /// the operation succeeds; otherwise, returns an <see cref="Error"/> describing the failure.</returns>
    Either<Error, Unit> Set<T>(string key, params (string, T)[] pairs);

    /// <inheritdoc cref="Set{T}(string, string, T)"/>
    Task<Either<Error, Unit>> SetAsync<T>(string key, string field, T value);

    /// <inheritdoc cref="Set{T}(string, ValueTuple{string, T}[])"/>
    Task<Either<Error, Unit>> SetAsync<T>(string key, params (string, T)[] pairs);

    /// <summary>
    /// Retrieves the values associated with the specified key, if available.
    /// </summary>
    /// <typeparam name="T">The type of the values to retrieve.</typeparam>
    /// <param name="key">The key used to look up the values. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{Error, Option{T[]}}"/> containing either an error or an optional array of values. If the
    /// key is not found, the result will contain an empty option.</returns>
    Either<Error, Option<T[]>> GetValues<T>(string key);

    /// <inheritdoc cref="GetValues{T}(string)"/>
    Task<Either<Error, Option<T[]>>> GetValuesAsync<T>(string key);

    /// <summary>
    /// Retrieves all values associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the values to retrieve.</typeparam>
    /// <param name="key">The key used to look up the values. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{Error, Option{T}}"/> containing either an error or an optional array of key-value pairs. If
    /// successful, the result is an <see cref="Option{T}"/> containing an array of tuples, where each tuple consists of
    /// a string key and a value of type <typeparamref name="T"/>. If no values are found, the <see cref="Option{T}"/>
    /// will be empty.</returns>
    Either<Error, Option<(string, T)[]>> GetAll<T>(string key);

    /// <inheritdoc cref="GetAll{T}(string)"/>
    Task<Either<Error, Option<(string, T)[]>>> GetAllAsync<T>(string key);

    /// <summary>
    /// Retrieves the field keys associated with the specified key.
    /// </summary>
    /// <param name="key">The key for which field keys are to be retrieved. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/> if the operation fails,  or an
    /// <see cref="Option{T}"/> representing the field keys as a string array.  If no field keys are found, the <see
    /// cref="Option{T}"/> will be empty.</returns>
    Either<Error, Option<string[]>> GetFieldKeys(string key);

    /// <inheritdoc cref="GetFieldKeys(string)"/>
    Task<Either<Error, Option<string[]>>> GetFieldKeysAsync(string key);

}