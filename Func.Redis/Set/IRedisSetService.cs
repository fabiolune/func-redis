using TinyFp;

namespace Func.Redis.Set;
public interface IRedisSetService
{
    Either<Error, Unit> Add<T>(string key, T value);
    Task<Either<Error, Unit>> AddAsync<T>(string key, T value);
    Either<Error, Unit> Delete<T>(string key, T value);
    Either<Error, Unit> Delete<T>(string key, params T[] values);
    Task<Either<Error, Unit>> DeleteAsync<T>(string key, T value);
    Task<Either<Error, Unit>> DeleteAsync<T>(string key, params T[] values);
    Either<Error, T[]> Difference<T>(string key1, string key2);
    Task<Either<Error, T[]>> DifferenceAsync<T>(string key1, string key2);
    Either<Error, Option<T>[]> GetAll<T>(string key);
    Task<Either<Error, Option<T>[]>> GetAllAsync<T>(string key);
    Either<Error, T[]> Intersect<T>(string key1, string key2);
    Task<Either<Error, T[]>> IntersectAsync<T>(string key1, string key2);
    Either<Error, Option<T>> Pop<T>(string key);
    Task<Either<Error, Option<T>>> PopAsync<T>(string key);
    Either<Error, long> Size(string key);
    Task<Either<Error, long>> SizeAsync(string key);
    Either<Error, T[]> Union<T>(string key1, string key2);
    Task<Either<Error, T[]>> UnionAsync<T>(string key1, string key2);
}