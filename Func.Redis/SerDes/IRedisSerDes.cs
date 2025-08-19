namespace Func.Redis.SerDes;

public interface IRedisSerDes
{
    /// <summary>
    /// Deserializes the specified Redis value into an instance of the specified type.
    /// </summary>
    /// <remarks>This method attempts to convert the Redis value into the specified type <typeparamref
    /// name="T"/>.  If the conversion fails, the method returns an empty <see cref="Option{T}"/> instead of throwing an
    /// exception.</remarks>
    /// <typeparam name="T">The type to deserialize the Redis value into.</typeparam>
    /// <param name="value">The Redis value to deserialize. Must not be null or empty.</param>
    /// <returns>An <see cref="Option{T}"/> containing the deserialized object if successful;  otherwise, an empty <see
    /// cref="Option{T}"/> if the value cannot be deserialized.</returns>
    Option<T> Deserialize<T>(RedisValue value);

    /// <summary>
    /// Deserializes an array of <see cref="RedisValue"/> objects into an array of the specified type.
    /// </summary>
    /// <remarks>This method attempts to deserialize each <see cref="RedisValue"/> in the input array into the
    /// specified type. Ensure that the type <typeparamref name="T"/> is compatible with the data contained in the <see
    /// cref="RedisValue"/> objects.</remarks>
    /// <typeparam name="T">The type to which each <see cref="RedisValue"/> will be deserialized.</typeparam>
    /// <param name="values">An array of <see cref="RedisValue"/> objects to deserialize. Cannot be null.</param>
    /// <returns>An <see cref="Option{T[]}"/> containing the deserialized array of type <typeparamref name="T"/>. If
    /// deserialization fails, the <see cref="Option{T[]}"/> will represent an empty or error state.</returns>
    Option<T[]> Deserialize<T>(RedisValue[] values);

    /// <summary>
    /// Deserializes an array of hash entries into an array of key-value pairs, where the key is a string and the value
    /// is of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the value to deserialize each hash entry into.</typeparam>
    /// <param name="entries">An array of hash entries to be deserialized. Each entry represents a key-value pair.</param>
    /// <returns>An array of key-value pairs, where the key is a string and the value is of type <typeparamref name="T"/>.
    /// Returns an empty array if no entries are provided.</returns>
    Option<(string, T)[]> Deserialize<T>(HashEntry[] entries);

    /// <summary>
    /// Deserializes the specified Redis value into an object of the given type.
    /// </summary>
    /// <remarks>This method attempts to convert the Redis value into the specified type. If the conversion is
    /// not possible, the method returns an empty <see cref="Option{T}"/> rather than throwing an exception.</remarks>
    /// <param name="value">The Redis value to deserialize. Must not be null or empty.</param>
    /// <param name="type">The target type to deserialize the value into. Must not be null.</param>
    /// <returns>An <see cref="Option{T}"/> containing the deserialized object if successful; otherwise, an empty <see
    /// cref="Option{T}"/> if deserialization fails.</returns>
    Option<object> Deserialize(RedisValue value, Type type);

    /// <summary>
    /// Serializes the specified value into a Redis-compatible format.
    /// </summary>
    /// <remarks>This method converts the input value into a format suitable for storage in Redis.  Ensure
    /// that the type <typeparamref name="T"/> is supported by the serialization mechanism.</remarks>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="value">The value to serialize. Cannot be null.</param>
    /// <returns>A <see cref="RedisValue"/> representing the serialized form of the input value.</returns>
    RedisValue Serialize<T>(T value);
}