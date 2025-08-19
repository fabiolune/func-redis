
namespace Func.Redis.List;

public interface IRedisListService
{
    /// <summary>
    /// Appends a value to the specified key in the underlying data store.
    /// </summary>
    /// <remarks>The operation may fail if the key does not exist, the value is incompatible with the key, or
    /// if there is an issue with the underlying data store. Check the returned <see cref="Either{Error, Unit}"/> to
    /// determine the outcome of the operation.</remarks>
    /// <typeparam name="T">The type of the value to append.</typeparam>
    /// <param name="key">The key to which the value will be appended. Cannot be null or empty.</param>
    /// <param name="value">The value to append. Must be compatible with the type expected by the key.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation. Returns <see langword="Unit"/> if
    /// the operation succeeds, or an <see cref="Error"/> if the operation fails.</returns>
    Either<Error, Unit> Append<T>(string key, T value);

    /// <summary>
    /// Appends the specified values to the collection associated with the given key.
    /// </summary>
    /// <typeparam name="T">The type of the values to append.</typeparam>
    /// <param name="key">The key identifying the collection to which the values will be appended. Cannot be null or empty.</param>
    /// <param name="values">The values to append to the collection. Cannot be null.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation. Returns <see langword="Unit"/> if
    /// the operation succeeds; otherwise, returns an <see cref="Error"/> describing the failure.</returns>
    Either<Error, Unit> Append<T>(string key, params T[] values);

    /// <inheritdoc cref="Append{T}(string, T)" /> >
    Task<Either<Error, Unit>> AppendAsync<T>(string key, T value);

    /// <inheritdoc cref="Append{T}(string, T[])"/>
    Task<Either<Error, Unit>> AppendAsync<T>(string key, params T[] values);

    /// <summary>
    /// Retrieves the value associated with the specified key and index.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key used to identify the value. Cannot be null or empty.</param>
    /// <param name="index">The index used to locate the value. Must be a non-negative number.</param>
    /// <returns>An <see cref="Either{Error, Option{T}}"/> containing either an error or an optional value. If the operation
    /// succeeds, the result will contain an <see cref="Option{T}"/> representing the value. If the operation fails, the
    /// result will contain an <see cref="Error"/> describing the failure.</returns>
    Either<Error, Option<T>> Get<T>(string key, long index);

    /// <summary>
    /// Retrieves a range of values associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the values to retrieve.</typeparam>
    /// <param name="key">The key identifying the collection of values. Cannot be null or empty.</param>
    /// <param name="start">The starting index of the range to retrieve. Must be greater than or equal to 0.</param>
    /// <param name="stop">The ending index of the range to retrieve. Must be greater than or equal to <paramref name="start"/>.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/> if the operation fails, or an
    /// array of <see cref="Option{T}"/> representing the values in the specified range. The array will be empty if no
    /// values are found within the range.</returns>
    Either<Error, Option<T>[]> Get<T>(string key, long start, long stop);

    /// <inheritdoc cref="Get{T}(string, long)"/>
    Task<Either<Error, Option<T>>> GetAsync<T>(string key, long index);

    /// <inheritdoc cref="Get{T}(string, long, long)"/>
    Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, long start, long stop);

    /// <summary>
    /// Removes and retrieves the value associated with the specified key from the underlying storage.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key identifying the value to remove and retrieve. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing an <see cref="Error"/> if the operation fails,  or an <see
    /// cref="Option{T}"/> representing the retrieved value if successful.  The <see cref="Option{T}"/> will be empty if
    /// the key does not exist.</returns>
    Either<Error, Option<T>> Pop<T>(string key);

    /// <summary>
    /// Retrieves and removes up to the specified number of items associated with the given key.
    /// </summary>
    /// <typeparam name="T">The type of the items to retrieve.</typeparam>
    /// <param name="key">The key identifying the collection of items to pop. Cannot be null or empty.</param>
    /// <param name="count">The maximum number of items to retrieve. Must be greater than zero.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/> if the operation fails, or an
    /// array of <see cref="Option{T}"/> representing the retrieved items. The array will be empty if no items are
    /// available.</returns>
    Either<Error, Option<T>[]> Pop<T>(string key, long count);

    ///  <inheritdoc cref="Pop{T}(string)"/>
    Task<Either<Error, Option<T>>> PopAsync<T>(string key);

    /// <inheritdoc cref="Pop{T}(string, long)"/>
    Task<Either<Error, Option<T>[]>> PopAsync<T>(string key, long count);

    /// <summary>
    /// Adds the specified values to the beginning of the collection associated with the given key.
    /// </summary>
    /// <remarks>This method modifies the collection associated with the specified key by adding the values to
    /// the beginning.  If the key does not exist, a new collection may be created depending on the
    /// implementation.</remarks>
    /// <typeparam name="T">The type of the values to prepend.</typeparam>
    /// <param name="key">The key identifying the collection to which the values will be prepended. Cannot be null or empty.</param>
    /// <param name="values">The values to prepend to the collection. If no values are provided, the method performs no operation.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see langword="true"/> if
    /// the operation succeeds; otherwise, contains an <see cref="Error"/> describing the failure.</returns>
    Either<Error, Unit> Prepend<T>(string key, params T[] values);

    /// <summary>
    /// Adds a key-value pair to the beginning of a collection or data structure.
    /// </summary>
    /// <remarks>This method prepends the specified key-value pair, ensuring the key is unique within the
    /// collection. If the key already exists, the operation may fail and return an <see cref="Error"/>.</remarks>
    /// <typeparam name="T">The type of the value to associate with the specified key.</typeparam>
    /// <param name="key">The key to associate with the value. Cannot be null or empty.</param>
    /// <param name="value">The value to associate with the key.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation. Returns <see cref="Unit"/> if the
    /// operation succeeds, or an <see cref="Error"/> if the operation fails.</returns>
    Either<Error, Unit> Prepend<T>(string key, T value);

    /// <inheritdoc cref="Prepend{T}(string, T)"/>
    Task<Either<Error, Unit>> PrependAsync<T>(string key, params T[] values);

    /// <inheritdoc cref="Prepend{T}(string, T[])"/>
    Task<Either<Error, Unit>> PrependAsync<T>(string key, T value);

    /// <summary>
    /// Retrieves the value associated with the specified key and shifts it into an optional result.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key used to locate the value. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{Error, Option{T}}"/> containing either an error if the operation fails,  or an optional
    /// value if the key is found.</returns>
    Either<Error, Option<T>> Shift<T>(string key);

    /// <summary>
    /// Retrieves and removes up to the specified number of items associated with the given key.
    /// </summary>
    /// <typeparam name="T">The type of the items to retrieve.</typeparam>
    /// <param name="key">The key identifying the collection of items to shift. Cannot be null or empty.</param>
    /// <param name="count">The maximum number of items to retrieve and remove. Must be greater than zero.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/> if the operation fails,  or an
    /// array of <see cref="Option{T}"/> representing the retrieved items. The array will be empty if no items are
    /// available.</returns>
    Either<Error, Option<T>[]> Shift<T>(string key, long count);

    /// <inheritdoc cref="Shift{T}(string)"/>
    Task<Either<Error, Option<T>>> ShiftAsync<T>(string key);

    /// <inheritdoc cref="Shift{T}(string, long)"/>
    Task<Either<Error, Option<T>[]>> ShiftAsync<T>(string key, long count);

    /// <summary>
    /// Retrieves the size, in bytes, of the data associated with the specified key.
    /// </summary>
    /// <param name="key">The key identifying the data whose size is to be retrieved. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/> if the operation fails,  or a
    /// <see langword="long"/> representing the size of the data in bytes if successful.</returns>
    Either<Error, long> Size(string key);

    /// <inheritdoc cref="Size(string)"/>
    Task<Either<Error, long>> SizeAsync(string key);
}