namespace Func.Redis.SortedSet;

public interface IRedisSortedSetService
{
    /// <summary>
    /// Add values with scores to a sorted set key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns><see cref="Unit"/> or <see cref="Error"/></returns>
    Either<Error, Unit> Add<T>(string key, IEnumerable<(T Value, double Score)> values);
    
    /// <summary>
    /// Add a value with score to a sorted set key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns><see cref="Unit"/> or <see cref="Error"/></returns>
    Either<Error, Unit> Add<T>(string key, T value, double score);

    /// inheritdoc cref="Add{T}(string, IEnumerable{(T Value, double Score)})"/>
    Task<Either<Error, Unit>> AddAsync<T>(string key, IEnumerable<(T Value, double Score)> values);

    /// inheritdoc cref="Add{T}(string, T, double)"/>
    Task<Either<Error, Unit>> AddAsync<T>(string key, T value, double score);
    /// <summary>
    /// Decrement the score of a value in a sorted set key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="score"></param>
    /// <returns></returns>
    Either<Error, Unit> Decrement<T>(string key, T value, double score);

    /// <inheritdoc cref="Decrement{T}(string, T, double)"/>
    Task<Either<Error, Unit>> DecrementAsync<T>(string key, T value, double score);
 
    /// <summary>
    /// Increments the score associated with the specified key by the given value.
    /// </summary>
    /// <remarks>This method is typically used in scenarios where scores are tracked and updated dynamically,
    /// such as leaderboards or ranking systems. Ensure that the key exists and is valid before calling this method to
    /// avoid errors.</remarks>
    /// <typeparam name="T">The type of the value to be incremented. Typically a numeric type.</typeparam>
    /// <param name="key">The key identifying the item whose score is to be incremented. Cannot be null or empty.</param>
    /// <param name="value">The value to increment the score by. Must be compatible with the type parameter <typeparamref name="T"/>.</param>
    /// <param name="score">The amount by which the score is incremented. Must be a valid double value.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation. Returns <see langword="Unit"/> if
    /// the operation succeeds, or an <see cref="Error"/> if the operation fails.</returns>
    Either<Error, Unit> Increment<T>(string key, T value, double score);
 
    /// <inheritdoc cref="Intersect{T}(string[])"/>
    Task<Either<Error, Unit>> IncrementAsync<T>(string key, T value, double score);
 
    /// <summary>
    /// Retrieves the intersection of values associated with the specified keys.
    /// </summary>
    /// <typeparam name="T">The type of the values to be intersected.</typeparam>
    /// <param name="keys">An array of keys used to identify the values to intersect. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{Error, T[]}"/> containing either an error if the operation fails,  or an array of values of
    /// type <typeparamref name="T"/> that represent the intersection of the values associated with the provided keys.</returns>
    Either<Error, T[]> Intersect<T>(string[] keys);

    /// <inheritdoc cref="Intersect{T}(string[])"/>
    Task<Either<Error, T[]>> IntersectAsync<T>(string[] keys);
 
    /// <summary>
    /// Retrieves the length of the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key identifying the value whose length is to be retrieved. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/> if the operation fails,  or a
    /// <see langword="long"/> representing the length of the value if the operation succeeds.</returns>
    Either<Error, long> Length(string key);
 
    /// <summary>
    /// Retrieves the count of elements in a sorted set stored at the specified key,  where the elements' scores fall
    /// within the given range.
    /// </summary>
    /// <remarks>The score range is inclusive of both <paramref name="min"/> and
    /// <paramref name="max"/>.</remarks>
    /// <param name="key">The key identifying the sorted set. Cannot be null or empty.</param>
    /// <param name="min">The minimum score of the range. Elements with scores greater than or equal to this value are included.</param>
    /// <param name="max">The maximum score of the range. Elements with scores less than or equal to this value are included.</param>
    /// <returns>An <see cref="Either{Error, long}"/> containing either an error if the operation fails,  or the count of
    /// elements within the specified score range.</returns>
    Either<Error, long> LengthByScore(string key, double min, double max);
  
    /// <inheritdoc cref="Length(string)"/>
    Task<Either<Error, long>> LengthAsync(string key);
  
    /// <inheritdoc cref="LengthByScore(string, double, double)"/>
    Task<Either<Error, long>> LengthByScoreAsync(string key, double min, double max);
  
    /// <summary>
    /// Retrieves the length of a collection stored under the specified key, filtered by a range of values.
    /// </summary>
    /// <remarks>The method filters the collection based on the provided range [<paramref name="min"/>,
    /// <paramref name="max"/>].  Ensure that <paramref name="min"/> is less than or equal to <paramref name="max"/> to
    /// avoid unexpected results.</remarks>
    /// <typeparam name="T">The type of the values used to filter the collection.</typeparam>
    /// <param name="key">The key identifying the collection in the data store. Cannot be null or empty.</param>
    /// <param name="min">The minimum value of the range used to filter the collection.</param>
    /// <param name="max">The maximum value of the range used to filter the collection.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/> if the operation fails,  or a
    /// <see langword="long"/> representing the count of items in the collection that fall within the specified range.</returns>
    Either<Error, long> LengthByValue<T>(string key, T min, T max);
 
    /// <inheritdoc cref="LengthByValue{T}(string, T, T)"/>
    Task<Either<Error, long>> LengthByValueAsync<T>(string key, T min, T max);

    /// <summary>
    /// Retrieves the rank of a specified value within a collection associated with the given key.
    /// </summary>
    /// <remarks>The rank is determined based on the ordering of the collection associated with the specified
    /// key.      If the key does not exist or the value is not found, the method returns <see langword="None"/> within
    /// the result.</remarks>
    /// <typeparam name="T">The type of the value to rank. Must be comparable within the collection.</typeparam>
    /// <param name="key">The key identifying the collection in which the rank is calculated. Cannot be null or empty.</param>
    /// <param name="value">The value whose rank is to be determined. Must exist within the collection.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/> if the operation fails,      or
    /// an <see cref="Option{T}"/> representing the rank of the value as a zero-based index.      Returns <see
    /// langword="None"/> if the value is not found in the collection.</returns>
    Either<Error, Option<long>> Rank<T>(string key, T value);
  
    /// <inheritdoc cref="Rank{T}(string, T)"/>
    Task<Either<Error, Option<long>>> RankAsync<T>(string key, T value);
 
    /// <summary>
    /// Removes the specified values associated with the given key from the collection.
    /// </summary>
    /// <typeparam name="T">The type of the values to be removed.</typeparam>
    /// <param name="key">The key identifying the collection from which the values will be removed. Cannot be null or empty.</param>
    /// <param name="values">The values to be removed from the collection. Cannot be null.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see langword="Unit"/> if
    /// the removal is successful, or an <see cref="Error"/> if the operation fails.</returns>
    Either<Error, Unit> Remove<T>(string key, IEnumerable<T> values);
 
    /// <summary>
    /// Removes the specified value associated with the given key from the collection.
    /// </summary>
    /// <remarks>The method will fail if the specified key does not exist in the collection or if the value
    /// does not match the one associated with the key.</remarks>
    /// <typeparam name="T">The type of the value to be removed.</typeparam>
    /// <param name="key">The key identifying the value to be removed. Cannot be null or empty.</param>
    /// <param name="value">The value to be removed. Must match the value associated with the specified key.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see langword="Unit"/> if
    /// the removal is successful, or an <see cref="Error"/> if the operation fails.</returns>
    Either<Error, Unit> Remove<T>(string key, T value);
 
    /// <inheritdoc cref="Remove{T}(string, IEnumerable{T})"/>
    Task<Either<Error, Unit>> RemoveAsync<T>(string key, IEnumerable<T> values);
  
    /// <inheritdoc cref="Remove{T}(string, T)"/>
    Task<Either<Error, Unit>> RemoveAsync<T>(string key, T value);
 
    /// <summary>
    /// Removes all elements in the sorted set stored at the specified key with scores within the given range.
    /// </summary>
    /// <remarks>This method operates on a sorted set and removes elements whose scores fall within the
    /// specified range. The range is inclusive of both <paramref name="start"/> and <paramref name="stop"/>.</remarks>
    /// <param name="key">The key identifying the sorted set. Cannot be null or empty.</param>
    /// <param name="start">The inclusive lower bound of the score range.</param>
    /// <param name="stop">The inclusive upper bound of the score range.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see langword="Unit"/> if
    /// the operation succeeds, or an <see cref="Error"/> if it fails.</returns>
    Either<Error, Unit> RemoveRangeByScore(string key, double start, double stop);
 
    /// <inheritdoc cref="RemoveRangeByScore(string, double, double)"/>
    Task<Either<Error, Unit>> RemoveRangeByScoreAsync(string key, double start, double stop);
 
    /// <summary>
    /// Removes a range of values from a data store based on the specified key and value range.
    /// </summary>
    /// <remarks>The method removes all values within the specified range [<paramref name="min"/>, <paramref
    /// name="max"/>]  associated with the given <paramref name="key"/>. The range is inclusive of both
    /// bounds.</remarks>
    /// <typeparam name="T">The type of the values to compare. Must be comparable.</typeparam>
    /// <param name="key">The key identifying the data store or collection from which values will be removed. Cannot be null or empty.</param>
    /// <param name="min">The minimum value of the range to remove. Values less than this will not be affected.</param>
    /// <param name="max">The maximum value of the range to remove. Values greater than this will not be affected.</param>
    /// <returns>An <see cref="Either{Error, Unit}"/> indicating the result of the operation.  Returns <see langword="Unit"/> if
    /// the operation succeeds, or an <see cref="Error"/> if the operation fails.</returns>
    Either<Error, Unit> RemoveRangeByValue<T>(string key, T min, T max);

    /// <inheritdoc cref="RemoveRangeByValue{T}(string, T, T)"/>
    Task<Either<Error, Unit>> RemoveRangeByValueAsync<T>(string key, T min, T max);
 
    /// <summary>
    /// Calculates a score based on the provided key and value, returning either an error or an optional score.
    /// </summary>
    /// <typeparam name="T">The type of the value used in the scoring operation.</typeparam>
    /// <param name="key">A string representing the key used to identify the scoring context. Cannot be null or empty.</param>
    /// <param name="value">The value associated with the key, which influences the scoring calculation.</param>
    /// <returns>An <see cref="Either{Error, Option{double}}"/> containing either an <see cref="Error"/> if the operation fails, 
    /// or an <see cref="Option{double}"/> representing the calculated score. The score may be absent if no valid result
    /// is produced.</returns>
    Either<Error, Option<double>> Score<T>(string key, T value);
 
    /// <inheritdoc cref="Score{T}(string, T)"/>
    Task<Either<Error, Option<double>>> ScoreAsync<T>(string key, T value);

    /// <summary>
    /// Combines the values associated with the specified keys into a single array.
    /// </summary>
    /// <typeparam name="T">The type of the values to be combined.</typeparam>
    /// <param name="keys">An array of keys whose associated values will be combined. Cannot be null or empty.</param>
    /// <returns>An <see cref="Either{Error, T[]}"/> containing either an error if the operation fails,  or an array of values of
    /// type <typeparamref name="T"/> if the operation succeeds.</returns>
    Either<Error, T[]> Union<T>(string[] keys);

    /// <inheritdoc cref="Union{T}(string[])"/>
    Task<Either<Error, T[]>> UnionAsync<T>(string[] keys);
}