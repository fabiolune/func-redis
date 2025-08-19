using Func.Redis.Extensions;
using Microsoft.Extensions.Logging;
using TinyFp.Extensions;

namespace Func.Redis.SortedSet;
internal class LoggingRedisSortedSetService(
    ILogger<IRedisSortedSetService> logger,
    IRedisSortedSetService service) : IRedisSortedSetService
{
    private readonly IRedisSortedSetService _service = service;
    private readonly ILogger _logger = logger;
    private const string ComponentName = nameof(IRedisSortedSetService);

    public Either<Error, Unit> Add<T>(string key, IEnumerable<(T Value, double Score)> values) =>
        (key, values)
            .Tee(t => _logger.LogInformation("{Component}: adding items to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.Add(t.key, t.values))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Add<T>(string key, T value, double score) =>
        (key, value, score)
            .Tee(t => _logger.LogInformation("{Component}: adding item to \"{Key}\" with score {Score}", ComponentName, t.key, t.score))
            .Map(t => _service.Add(t.key, t.value, t.score))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> AddAsync<T>(string key, IEnumerable<(T Value, double Score)> values) =>
        (key, values)
            .Tee(t => _logger.LogInformation("{Component}: async adding items to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.AddAsync(t.key, t.values))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> AddAsync<T>(string key, T value, double score) =>
        (key, value, score)
            .Tee(t => _logger.LogInformation("{Component}: async adding item to \"{Key}\" with score {Score}", ComponentName, t.key, t.score))
            .Map(t => _service.AddAsync(t.key, t.value, t.score))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Decrement<T>(string key, T value, double score) =>
        (key, value, score)
            .Tee(t => _logger.LogInformation("{Component}: decrementing item \"{Value}\" in \"{Key}\" by {Score}", ComponentName, t.value, t.key, t.score))
            .Map(t => _service.Decrement(t.key, t.value, t.score))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> DecrementAsync<T>(string key, T value, double score) =>
        (key, value, score)
            .Tee(t => _logger.LogInformation("{Component}: async decrementing item \"{Value}\" in \"{Key}\" by {Score}", ComponentName, t.value, t.key, t.score))
            .Map(t => _service.DecrementAsync(t.key, t.value, t.score))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Increment<T>(string key, T value, double score) =>
        (key, value, score)
            .Tee(t => _logger.LogInformation("{Component}: incrementing item \"{Value}\" in \"{Key}\" by {Score}", ComponentName, t.value, t.key, t.score))
            .Map(t => _service.Increment(t.key, t.value, t.score))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> IncrementAsync<T>(string key, T value, double score) =>
        (key, value, score)
            .Tee(t => _logger.LogInformation("{Component}: async incrementing item \"{Value}\" in \"{Key}\" by {Score}", ComponentName, t.value, t.key, t.score))
            .Map(t => _service.IncrementAsync(t.key, t.value, t.score))
            .TeeLog(_logger, ComponentName);

    public Either<Error, T[]> Intersect<T>(string[] keys) =>
        keys
            .Tee(k => _logger.LogInformation("{Component}: intersecting keys [{Keys}]", ComponentName, string.Join(", ", k)))
            .Map(_service.Intersect<T>)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, T[]>> IntersectAsync<T>(string[] keys) =>
        keys
            .Tee(k => _logger.LogInformation("{Component}: async intersecting keys [{Keys}]", ComponentName, string.Join(", ", k)))
            .Map(_service.IntersectAsync<T>)
            .TeeLog(_logger, ComponentName);

    public Either<Error, long> Length(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: getting length of sorted set at \"{Key}\"", ComponentName, k))
            .Map(_service.Length)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, long>> LengthAsync(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: async getting length of sorted set at \"{Key}\"", ComponentName, k))
            .Map(_service.LengthAsync)
            .TeeLog(_logger, ComponentName);

    public Either<Error, long> LengthByScore(string key, double min, double max) =>
        (key, min, max)
            .Tee(t => _logger.LogInformation("{Component}: getting length of sorted set at \"{Key}\" with score range [{Min}, {Max}]", ComponentName, t.key, t.min, t.max))
            .Map(t => _service.LengthByScore(t.key, t.min, t.max))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, long>> LengthByScoreAsync(string key, double min, double max) =>
        (key, min, max)
            .Tee(t => _logger.LogInformation("{Component}: async getting length of sorted set at \"{Key}\" with score range [{Min}, {Max}]", ComponentName, t.key, t.min, t.max))
            .Map(t => _service.LengthByScoreAsync(t.key, t.min, t.max))
            .TeeLog(_logger, ComponentName);

    public Either<Error, long> LengthByValue<T>(string key, T min, T max) =>
        (key, min, max)
            .Tee(t => _logger.LogInformation("{Component}: getting length of sorted set at \"{Key}\" with value range [{Min}, {Max}]", ComponentName, t.key, t.min, t.max))
            .Map(t => _service.LengthByValue(t.key, t.min, t.max))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, long>> LengthByValueAsync<T>(string key, T min, T max) =>
        (key, min, max)
            .Tee(t => _logger.LogInformation("{Component}: async getting length of sorted set at \"{Key}\" with value range [{Min}, {Max}]", ComponentName, t.key, t.min, t.max))
            .Map(t => _service.LengthByValueAsync(t.key, t.min, t.max))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<long>> Rank<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: getting rank of item in sorted set at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.Rank(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<long>>> RankAsync<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: async getting rank of item in sorted set at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.RankAsync(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Remove<T>(string key, IEnumerable<T> values) =>
        (key, values)
            .Tee(t => _logger.LogInformation("{Component}: removing items from sorted set at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.Remove(t.key, t.values))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Remove<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: removing item from sorted set at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.Remove(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> RemoveAsync<T>(string key, IEnumerable<T> values) =>
        (key, values)
            .Tee(t => _logger.LogInformation("{Component}: async removing items from sorted set at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.RemoveAsync(t.key, t.values))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> RemoveAsync<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: async removing item from sorted set at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.RemoveAsync(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> RemoveRangeByScore(string key, double start, double stop) =>
        (key, start, stop)
            .Tee(t => _logger.LogInformation("{Component}: removing range by score from sorted set at \"{Key}\" with range [{Start}, {Stop}]", ComponentName, t.key, t.start, t.stop))
            .Map(t => _service.RemoveRangeByScore(t.key, t.start, t.stop))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> RemoveRangeByScoreAsync(string key, double start, double stop) =>
        (key, start, stop)
            .Tee(t => _logger.LogInformation("{Component}: async removing range by score from sorted set at \"{Key}\" with range [{Start}, {Stop}]", ComponentName, t.key, t.start, t.stop))
            .Map(t => _service.RemoveRangeByScoreAsync(t.key, t.start, t.stop))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> RemoveRangeByValue<T>(string key, T min, T max) =>
        (key, min, max)
            .Tee(t => _logger.LogInformation("{Component}: removing range by value from sorted set at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.RemoveRangeByValue(t.key, t.min, t.max))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> RemoveRangeByValueAsync<T>(string key, T min, T max) =>
        (key, min, max)
            .Tee(t => _logger.LogInformation("{Component}: async removing range by value from sorted set at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.RemoveRangeByValueAsync(t.key, t.min, t.max))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<double>> Score<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: getting score of item in sorted set at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.Score(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<double>>> ScoreAsync<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: async getting score of item in sorted set at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.ScoreAsync(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Either<Error, T[]> Union<T>(string[] keys) =>
        keys
            .Tee(k => _logger.LogInformation("{Component}: union of keys [{Keys}]", ComponentName, string.Join(", ", k)))
            .Map(_service.Union<T>)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, T[]>> UnionAsync<T>(string[] keys) =>
        keys
            .Tee(k => _logger.LogInformation("{Component}: async union of keys [{Keys}]", ComponentName, string.Join(", ", k)))
            .Map(_service.UnionAsync<T>)
            .TeeLog(_logger, ComponentName);

    public Either<Error, T[]> RangeByScore<T>(string key, double min, double max) =>
        (key, min, max)
            .Tee(t => _logger.LogInformation("{Component}: getting range by score from sorted set at \"{Key}\" with range [{Min}, {Max}]", ComponentName, t.key, t.min, t.max))
            .Map(t => _service.RangeByScore<T>(t.key, t.min, t.max))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, T[]>> RangeByScoreAsync<T>(string key, double min, double max) =>
        (key, min, max)
            .Tee(t => _logger.LogInformation("{Component}: async getting range by score from sorted set at \"{Key}\" with range [{Min}, {Max}]", ComponentName, t.key, t.min, t.max))
            .Map(t => _service.RangeByScoreAsync<T>(t.key, t.min, t.max))
            .TeeLog(_logger, ComponentName);
}
