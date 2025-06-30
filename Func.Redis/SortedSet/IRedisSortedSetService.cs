namespace Func.Redis.SortedSet;

public interface IRedisSortedSetService
{
    Either<Error, Unit> Add<T>(string key, IEnumerable<(T Value, double Score)> values);
    Either<Error, Unit> Add<T>(string key, T value, double score);
    Task<Either<Error, Unit>> AddAsync<T>(string key, IEnumerable<(T Value, double Score)> values);
    Task<Either<Error, Unit>> AddAsync<T>(string key, T value, double score);
    Either<Error, Unit> Decrement<T>(string key, T value, double score);
    Task<Either<Error, Unit>> DecrementAsync<T>(string key, T value, double score);
    Either<Error, Unit> Increment<T>(string key, T value, double score);
    Task<Either<Error, Unit>> IncrementAsync<T>(string key, T value, double score);
    Either<Error, T[]> Intersect<T>(string[] keys);
    Task<Either<Error, T[]>> IntersectAsync<T>(string[] keys);
    Either<Error, long> Length(string key);
    Either<Error, long> Length(string key, double min, double max);
    Task<Either<Error, long>> LengthAsync(string key);
    Task<Either<Error, long>> LengthAsync(string key, double min, double max);
    Either<Error, long> LengthByValue<T>(string key, T min, T max);
    Task<Either<Error, long>> LengthByValueAsync<T>(string key, T min, T max);
    Either<Error, Option<long>> Rank<T>(string key, T value);
    Task<Either<Error, Option<long>>> RankAsync<T>(string key, T value);
    Either<Error, Unit> Remove<T>(string key, IEnumerable<T> values);
    Either<Error, Unit> Remove<T>(string key, T value);
    Task<Either<Error, Unit>> RemoveAsync<T>(string key, IEnumerable<T> values);
    Task<Either<Error, Unit>> RemoveAsync<T>(string key, T value);
    Either<Error, Unit> RemoveRangeByScore(string key, double start, double stop);
    Task<Either<Error, Unit>> RemoveRangeByScoreAsync(string key, double start, double stop);
    Either<Error, Unit> RemoveRangeByValue<T>(string key, T min, T max);
    Task<Either<Error, Unit>> RemoveRangeByValueAsync<T>(string key, T min, T max);
    Either<Error, Option<double>> Score<T>(string key, T value);
    Task<Either<Error, Option<double>>> ScoreAsync<T>(string key, T value);
    Either<Error, T[]> Union<T>(string[] keys);
    Task<Either<Error, T[]>> UnionAsync<T>(string[] keys);
}