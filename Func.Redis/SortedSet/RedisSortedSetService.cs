using Func.Redis.SerDes;
using TinyFp.Extensions;
using static Func.Redis.Utils.FunctionUtilities;

namespace Func.Redis.SortedSet;
public class RedisSortedSetService(
    ISourcesProvider sourcesProvider,
    IRedisSerDes serDes) : IRedisSortedSetService
{
    private readonly IDatabase _database = sourcesProvider.GetDatabase();
    private readonly IRedisSerDes _serDes = serDes;

    private static readonly Error AddError = Error.New("Redis ZADD Error");
    private static readonly Error RemoveError = Error.New("Redis ZREM Error");

    public Either<Error, Unit> Add<T>(string key, T value, double score) =>
        Wrap(() => _database.SortedSetAdd(key, _serDes.Serialize(value), score), AddError);

    public Either<Error, Unit> Add<T>(string key, IEnumerable<(T Value, double Score)> values) =>
        Wrap(() => _database.SortedSetAdd(key, [.. values.Select(t => new SortedSetEntry(_serDes.Serialize(t.Value), t.Score))]), ToUnit);

    public Either<Error, Unit> Remove<T>(string key, T value) =>
        Wrap(() => _database.SortedSetRemove(key, _serDes.Serialize(value)), RemoveError);

    public Either<Error, Unit> Remove<T>(string key, IEnumerable<T> values) =>
        Wrap(() => _database.SortedSetRemove(key, [.. values.Select(_serDes.Serialize)]), ToUnit);

    public Either<Error, Unit> RemoveRangeByScore(string key, double start, double stop) =>
        Wrap(() => _database.SortedSetRemoveRangeByScore(key, start, stop), ToUnit);

    public Either<Error, Unit> RemoveRangeByValue<T>(string key, T min, T max) =>
        Wrap(() => _database.SortedSetRemoveRangeByValue(key, _serDes.Serialize(min), _serDes.Serialize(max)), ToUnit);

    public Either<Error, Unit> Increment<T>(string key, T value, double score) =>
        Wrap(() => _database.SortedSetIncrement(key, _serDes.Serialize(value), score), ToUnit);

    public Either<Error, Unit> Decrement<T>(string key, T value, double score) =>
        Wrap(() => _database.SortedSetDecrement(key, _serDes.Serialize(value), score), ToUnit);

    public Either<Error, long> Length(string key) =>
        Wrap(() => _database.SortedSetLength(key));

    public Either<Error, long> LengthByScore(string key, double min, double max) =>
        Wrap(() => _database.SortedSetLength(key, min, max));

    public Either<Error, long> LengthByValue<T>(string key, T min, T max) =>
        Wrap(() => _database.SortedSetLengthByValue(key, _serDes.Serialize(min), _serDes.Serialize(max)));

    public Either<Error, Option<long>> Rank<T>(string key, T value) =>
        Wrap(() => _database.SortedSetRank(key, _serDes.Serialize(value)).ToOption(v => v!.Value, v => !v.HasValue));

    public Either<Error, Option<double>> Score<T>(string key, T value) =>
        Wrap(() => _database.SortedSetScore(key, _serDes.Serialize(value)).ToOption(v => v!.Value, v => !v.HasValue));

    public Either<Error, T[]> Intersect<T>(string[] keys) =>
        Combine<T>(keys, SetOperation.Intersect);

    public Either<Error, T[]> Union<T>(string[] keys) =>
        Combine<T>(keys, SetOperation.Union);

    private Either<Error, T[]> Combine<T>(string[] keys, SetOperation operation) =>
        Wrap(() => _database.SortedSetCombine(operation, [.. keys.Select(k => (RedisKey)k)]).Select(_serDes.Deserialize<T>).Filter().ToArray());

    public Task<Either<Error, Unit>> AddAsync<T>(string key, T value, double score) =>
        WrapAsync(() => _database.SortedSetAddAsync(key, _serDes.Serialize(value), score), AddError);

    public Task<Either<Error, Unit>> AddAsync<T>(string key, IEnumerable<(T Value, double Score)> values) =>
        WrapAsync(() => _database.SortedSetAddAsync(key, [.. values.Select(t => new SortedSetEntry(_serDes.Serialize(t.Value), t.Score))]), ToUnit);

    public Task<Either<Error, Unit>> RemoveAsync<T>(string key, T value) =>
        WrapAsync(() => _database.SortedSetRemoveAsync(key, _serDes.Serialize(value)), RemoveError);

    public Task<Either<Error, Unit>> RemoveAsync<T>(string key, IEnumerable<T> values) =>
        WrapAsync(() => _database.SortedSetRemoveAsync(key, [.. values.Select(_serDes.Serialize)]), ToUnit);

    public Task<Either<Error, Unit>> RemoveRangeByScoreAsync(string key, double start, double stop) =>
        WrapAsync(() => _database.SortedSetRemoveRangeByScoreAsync(key, start, stop), ToUnit);

    public Task<Either<Error, Unit>> RemoveRangeByValueAsync<T>(string key, T min, T max) =>
        WrapAsync(() => _database.SortedSetRemoveRangeByValueAsync(key, _serDes.Serialize(min), _serDes.Serialize(max)), ToUnit);

    public Task<Either<Error, Unit>> IncrementAsync<T>(string key, T value, double score) =>
        WrapAsync(() => _database.SortedSetIncrementAsync(key, _serDes.Serialize(value), score), ToUnit);

    public Task<Either<Error, Unit>> DecrementAsync<T>(string key, T value, double score) =>
        WrapAsync(() => _database.SortedSetDecrementAsync(key, _serDes.Serialize(value), score), ToUnit);

    public Task<Either<Error, long>> LengthAsync(string key) =>
        WrapAsync(() => _database.SortedSetLengthAsync(key));

    public Task<Either<Error, long>> LengthByScoreAsync(string key, double min, double max) =>
        WrapAsync(() => _database.SortedSetLengthAsync(key, min, max));

    public Task<Either<Error, long>> LengthByValueAsync<T>(string key, T min, T max) =>
        WrapAsync(() => _database.SortedSetLengthByValueAsync(key, _serDes.Serialize(min), _serDes.Serialize(max)));

    public Task<Either<Error, Option<long>>> RankAsync<T>(string key, T value) =>
        WrapAsync(() => _database.SortedSetRankAsync(key, _serDes.Serialize(value)).ToOptionAsync(v => v!.Value, v => !v.HasValue));

    public Task<Either<Error, Option<double>>> ScoreAsync<T>(string key, T value) =>
        WrapAsync(() => _database.SortedSetScoreAsync(key, _serDes.Serialize(value)).ToOptionAsync(v => v!.Value, v => !v.HasValue));

    public Task<Either<Error, T[]>> IntersectAsync<T>(string[] keys) =>
        CombineAsync<T>(keys, SetOperation.Intersect);

    public Task<Either<Error, T[]>> UnionAsync<T>(string[] keys) =>
        CombineAsync<T>(keys, SetOperation.Union);

    private Task<Either<Error, T[]>> CombineAsync<T>(string[] keys, SetOperation operation) =>
        WrapUnsafeAsync(() => _database.SortedSetCombineAsync(operation, [.. keys.Select(k => (RedisKey)k)]), vs => vs.Select(_serDes.Deserialize<T>).Filter().ToArray());
}
