
namespace Func.Redis;

public interface IRedisService
{
    /// <summary>
    /// Executes the specified function within the context of a database operation and returns the result.
    /// </summary>
    /// <typeparam name="T">The type of the result produced by the function.</typeparam>
    /// <param name="exec">A function that takes an <see cref="IDatabase"/> instance and performs the desired operation.</param>
    /// <returns>An <see cref="Either{Error, T}"/> containing either the result of the operation or an error if the operation
    /// fails.</returns>
    Either<Error, T> Execute<T>(Func<IDatabase, T> exec);
    
    /// <summary>
    /// Executes a database operation and maps the result to a specified output type.
    /// </summary>
    /// <remarks>This method provides a mechanism to execute a database operation and process its result in a
    /// functional style. The <paramref name="exec"/> function is responsible for interacting with the database, while
    /// the <paramref name="map"/>  function allows transformation of the intermediate result into the desired
    /// output.</remarks>
    /// <typeparam name="TIn">The type of the intermediate result produced by the database operation.</typeparam>
    /// <typeparam name="TOut">The type of the final result after mapping.</typeparam>
    /// <param name="exec">A function that performs the database operation and returns a result of type <typeparamref name="TIn"/>.</param>
    /// <param name="map">A function that transforms the intermediate result of type <typeparamref name="TIn"/> into the final result of
    /// type <typeparamref name="TOut"/>.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing either an <see cref="Error"/> if the operation fails,  or the
    /// mapped result of type <typeparamref name="TOut"/> if the operation succeeds.</returns>
    Either<Error, TOut> Execute<TIn, TOut>(Func<IDatabase, TIn> exec, Func<TIn, TOut> map);
    
    /// <inheritdoc cref="Execute{T}(Func{IDatabase, T})"/>/>
    Task<Either<Error, T>> ExecuteAsync<T>(Func<IDatabase, Task<T>> exec);
    
    /// <inheritdoc cref="Execute{TIn, TOut}(Func{IDatabase, TIn}, Func{TIn, TOut})"/>
    Task<Either<Error, TOut>> ExecuteAsync<TIn, TOut>(Func<IDatabase, Task<TIn>> exec, Func<TIn, TOut> map);
    
    /// <summary>
    /// Executes an asynchronous operation on a database and maps the result to a specified output type.
    /// </summary>
    /// <typeparam name="TIn">The type of the intermediate result produced by the database operation.</typeparam>
    /// <typeparam name="TOut">The type of the final result after mapping.</typeparam>
    /// <param name="exec">A function that performs the database operation and returns a task producing the intermediate result.</param>
    /// <param name="map">A function that transforms the intermediate result into the final output type.</param>
    /// <returns>A task that represents the asynchronous operation. The result is an <see cref="Either{TLeft, TRight}"/>
    /// containing either an <see cref="Error"/> if the operation fails, or the mapped result of type <typeparamref
    /// name="TOut"/> if successful.</returns>
    Task<Either<Error, TOut>> ExecuteUnsafeAsync<TIn, TOut>(Func<IDatabase, Task<TIn>> exec, Func<TIn, TOut> map);
}