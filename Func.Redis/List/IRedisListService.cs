
namespace Func.Redis.List;

public interface IRedisListService
{
    Either<Error, Unit> Append<T>(string key, T value);
    Either<Error, Unit> Append<T>(string key, params T[] values);
    Task<Either<Error, Unit>> AppendAsync<T>(string key, T value);
    Task<Either<Error, Unit>> AppendAsync<T>(string key, params T[] values);
    Either<Error, Option<T>> Get<T>(string key, long index);
    Either<Error, Option<T>[]> Get<T>(string key, long start, long stop);
    Task<Either<Error, Option<T>>> GetAsync<T>(string key, long index);
    Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, long start, long stop);
    Either<Error, Option<T>> Pop<T>(string key);
    Either<Error, Option<T>[]> Pop<T>(string key, long count);
    Task<Either<Error, Option<T>>> PopAsync<T>(string key);
    Task<Either<Error, Option<T>[]>> PopAsync<T>(string key, long count);
    Either<Error, Unit> Prepend<T>(string key, params T[] values);
    Either<Error, Unit> Prepend<T>(string key, T value);
    Task<Either<Error, Unit>> PrependAsync<T>(string key, params T[] values);
    Task<Either<Error, Unit>> PrependAsync<T>(string key, T value);
    Either<Error, Option<T>> Shift<T>(string key);
    Either<Error, Option<T>[]> Shift<T>(string key, long count);
    Task<Either<Error, Option<T>>> ShiftAsync<T>(string key);
    Task<Either<Error, Option<T>[]>> ShiftAsync<T>(string key, long count);
    Either<Error, long> Size(string key);
    Task<Either<Error, long>> SizeAsync(string key);
}