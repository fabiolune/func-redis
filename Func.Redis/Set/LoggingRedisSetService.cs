using Func.Redis.Extensions;
using Microsoft.Extensions.Logging;

namespace Func.Redis.Set;
public class LoggingRedisSetService(
    ILogger logger,
    IRedisSetService service
    ) : IRedisSetService
{
    private readonly IRedisSetService _service = service;
    private readonly ILogger _logger = logger;
    private const string ComponentName = nameof(IRedisSetService);

    public Either<Error, Unit> Add<T>(string key, T value) =>
        _service
            .Add(key, value)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Add<T>(string key, params T[] values) =>
        _service
            .Add(key, values)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> AddAsync<T>(string key, T value) =>
        _service
            .AddAsync(key, value)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> AddAsync<T>(string key, params T[] values) =>
        _service
            .AddAsync(key, values)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Delete<T>(string key, T value) =>
        _service
            .Delete(key, value)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Delete<T>(string key, params T[] values) =>
        _service
            .Delete(key, values)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> DeleteAsync<T>(string key, T value) =>
        _service
            .DeleteAsync(key, value)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> DeleteAsync<T>(string key, params T[] values) =>
        _service
            .DeleteAsync(key, values)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>> Pop<T>(string key) =>
        _service
            .Pop<T>(key)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>>> PopAsync<T>(string key) =>
        _service
            .PopAsync<T>(key)
            .TeeLog(_logger, ComponentName);

    public Either<Error, T[]> Difference<T>(string key1, string key2) =>
        _service
            .Difference<T>(key1, key2)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, T[]>> DifferenceAsync<T>(string key1, string key2) =>
        _service
            .DifferenceAsync<T>(key1, key2)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>[]> GetAll<T>(string key) =>
        _service
            .GetAll<T>(key)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>[]>> GetAllAsync<T>(string key) =>
        _service
            .GetAllAsync<T>(key)
            .TeeLog(_logger, ComponentName);

    public Either<Error, T[]> Intersect<T>(string key1, string key2) =>
        _service
            .Intersect<T>(key1, key2)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, T[]>> IntersectAsync<T>(string key1, string key2) =>
        _service
            .IntersectAsync<T>(key1, key2)
            .TeeLog(_logger, ComponentName);

    public Either<Error, long> Size(string key) =>
        _service
            .Size(key)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, long>> SizeAsync(string key) =>
        _service
            .SizeAsync(key)
            .TeeLog(_logger, ComponentName);

    public Either<Error, T[]> Union<T>(string key1, string key2) =>
        _service
            .Union<T>(key1, key2)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, T[]>> UnionAsync<T>(string key1, string key2) =>
        _service
            .UnionAsync<T>(key1, key2)
            .TeeLog(_logger, ComponentName);
}
