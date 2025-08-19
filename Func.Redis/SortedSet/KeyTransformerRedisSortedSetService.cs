namespace Func.Redis.SortedSet;

/// <exclude />
public class KeyTransformerRedisSortedSetService(
    IRedisSortedSetService service,
    Func<string, string> keyMapper) : IRedisSortedSetService
{
    private readonly Func<string, string> _keyMapper = keyMapper;
    private readonly IRedisSortedSetService _service = service;

    public Either<Error, Unit> Add<T>(string key, IEnumerable<(T Value, double Score)> values) => _service.Add(_keyMapper(key), values);
    public Either<Error, Unit> Add<T>(string key, T value, double score) => _service.Add(_keyMapper(key), value, score);
    public Task<Either<Error, Unit>> AddAsync<T>(string key, IEnumerable<(T Value, double Score)> values) => _service.AddAsync(_keyMapper(key), values);
    public Task<Either<Error, Unit>> AddAsync<T>(string key, T value, double score) => _service.AddAsync(_keyMapper(key), value, score);
    public Either<Error, Unit> Decrement<T>(string key, T value, double score) => _service.Decrement(_keyMapper(key), value, score);
    public Task<Either<Error, Unit>> DecrementAsync<T>(string key, T value, double score) => _service.DecrementAsync(_keyMapper(key), value, score);
    public Either<Error, Unit> Increment<T>(string key, T value, double score) => _service.Increment(_keyMapper(key), value, score);
    public Task<Either<Error, Unit>> IncrementAsync<T>(string key, T value, double score) => _service.IncrementAsync(_keyMapper(key), value, score);
    public Either<Error, T[]> Intersect<T>(string[] keys) => _service.Intersect<T>([.. keys.Select(_keyMapper)]);
    public Task<Either<Error, T[]>> IntersectAsync<T>(string[] keys) => _service.IntersectAsync<T>([.. keys.Select(_keyMapper)]);
    public Either<Error, long> Length(string key) => _service.Length(_keyMapper(key));
    public Task<Either<Error, long>> LengthAsync(string key) => _service.LengthAsync(_keyMapper(key));
    public Either<Error, long> LengthByScore(string key, double min, double max) => _service.LengthByScore(_keyMapper(key), min, max);
    public Task<Either<Error, long>> LengthByScoreAsync(string key, double min, double max) => _service.LengthByScoreAsync(_keyMapper(key), min, max);
    public Either<Error, long> LengthByValue<T>(string key, T min, T max) => _service.LengthByValue(_keyMapper(key), min, max);
    public Task<Either<Error, long>> LengthByValueAsync<T>(string key, T min, T max) => _service.LengthByValueAsync(_keyMapper(key), min, max);
    public Either<Error, T[]> RangeByScore<T>(string key, double min, double max) => _service.RangeByScore<T>(_keyMapper(key), min, max);
    public Task<Either<Error, T[]>> RangeByScoreAsync<T>(string key, double min, double max) => _service.RangeByScoreAsync<T>(_keyMapper(key), min, max);
    public Either<Error, Option<long>> Rank<T>(string key, T value) => _service.Rank(_keyMapper(key), value);
    public Task<Either<Error, Option<long>>> RankAsync<T>(string key, T value) => _service.RankAsync(_keyMapper(key), value);
    public Either<Error, Unit> Remove<T>(string key, IEnumerable<T> values) => _service.Remove(_keyMapper(key), values);
    public Either<Error, Unit> Remove<T>(string key, T value) => _service.Remove(_keyMapper(key), value);
    public Task<Either<Error, Unit>> RemoveAsync<T>(string key, IEnumerable<T> values) => _service.RemoveAsync(_keyMapper(key), values);
    public Task<Either<Error, Unit>> RemoveAsync<T>(string key, T value) => _service.RemoveAsync(_keyMapper(key), value);
    public Either<Error, Unit> RemoveRangeByScore(string key, double start, double stop) => _service.RemoveRangeByScore(_keyMapper(key), start, stop);
    public Task<Either<Error, Unit>> RemoveRangeByScoreAsync(string key, double start, double stop) => _service.RemoveRangeByScoreAsync(_keyMapper(key), start, stop);
    public Either<Error, Unit> RemoveRangeByValue<T>(string key, T min, T max) => _service.RemoveRangeByValue(_keyMapper(key), min, max);
    public Task<Either<Error, Unit>> RemoveRangeByValueAsync<T>(string key, T min, T max) => _service.RemoveRangeByValueAsync(_keyMapper(key), min, max);
    public Either<Error, Option<double>> Score<T>(string key, T value) => _service.Score(_keyMapper(key), value);
    public Task<Either<Error, Option<double>>> ScoreAsync<T>(string key, T value) => _service.ScoreAsync(_keyMapper(key), value);
    public Either<Error, T[]> Union<T>(string[] keys) => _service.Union<T>([.. keys.Select(_keyMapper)]);
    public Task<Either<Error, T[]>> UnionAsync<T>(string[] keys) => _service.UnionAsync<T>([.. keys.Select(_keyMapper)]);
}
