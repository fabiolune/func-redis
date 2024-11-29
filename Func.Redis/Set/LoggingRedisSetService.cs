using Func.Redis.Extensions;
using Microsoft.Extensions.Logging;
using TinyFp.Extensions;

namespace Func.Redis.Set;
public class LoggingRedisSetService(
    ILogger logger,
    IRedisSetService service) : IRedisSetService
{
    private readonly IRedisSetService _service = service;
    private readonly ILogger _logger = logger;
    private const string ComponentName = nameof(IRedisSetService);

    public Either<Error, Unit> Add<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: adding item to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.Add(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Add<T>(string key, params T[] values) =>
        (key, values)
            .Tee(t => _logger.LogInformation("{Component}: adding items to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.Add(t.key, t.values))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> AddAsync<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: async adding item to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.AddAsync(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> AddAsync<T>(string key, params T[] values) =>
        (key, values)
            .Tee(t => _logger.LogInformation("{Component}: async adding items to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.AddAsync(t.key, t.values))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Delete<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: deleting value at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.Delete(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Delete<T>(string key, params T[] values) =>
        (key, values)
            .Tee(t => _logger.LogInformation("{Component}: deleting values at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.Delete(t.key, t.values))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> DeleteAsync<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: async deleting value at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.DeleteAsync(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> DeleteAsync<T>(string key, params T[] values) =>
        (key, values)
            .Tee(t => _logger.LogInformation("{Component}: async deleting values at \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.DeleteAsync(t.key, t.values))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>> Pop<T>(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: popping item from \"{Key}\"", ComponentName, k))
            .Map(_service.Pop<T>)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>>> PopAsync<T>(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: async popping item from \"{Key}\"", ComponentName, k))
            .Map(_service.PopAsync<T>)
            .TeeLog(_logger, ComponentName);

    public Either<Error, T[]> Difference<T>(string key1, string key2) =>
        (key1, key2)
            .Tee(t => _logger.LogInformation("{Component}: getting difference between \"{Key1}\" and \"{Key2}\"", ComponentName, t.key1, t.key2))
            .Map(t => _service.Difference<T>(t.key1, t.key2))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, T[]>> DifferenceAsync<T>(string key1, string key2) =>
        (key1, key2)
            .Tee(t => _logger.LogInformation("{Component}: async getting difference between \"{Key1}\" and \"{Key2}\"", ComponentName, t.key1, t.key2))
            .Map(t => _service.DifferenceAsync<T>(t.key1, t.key2))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>[]> GetAll<T>(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: getting all items for \"{Key}\"", ComponentName, k))
            .Map(_service.GetAll<T>)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>[]>> GetAllAsync<T>(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: async getting all items for \"{Key}\"", ComponentName, k))
            .Map(_service.GetAllAsync<T>)
            .TeeLog(_logger, ComponentName);

    public Either<Error, T[]> Intersect<T>(string key1, string key2) =>
        (key1, key2)
            .Tee(t => _logger.LogInformation("{Component}: getting intersection between \"{Key1}\" and \"{Key2}\"", ComponentName, t.key1, t.key2))
            .Map(t => _service.Intersect<T>(t.key1, t.key2))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, T[]>> IntersectAsync<T>(string key1, string key2) =>
        (key1, key2)
            .Tee(t => _logger.LogInformation("{Component}: async getting intersection between \"{Key1}\" and \"{Key2}\"", ComponentName, t.key1, t.key2))
            .Map(t => _service.IntersectAsync<T>(t.key1, t.key2))
            .TeeLog(_logger, ComponentName);

    public Either<Error, long> Size(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: getting size for \"{Key}\"", ComponentName, k))
            .Map(_service.Size)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, long>> SizeAsync(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: async getting size for \"{Key}\"", ComponentName, k))
            .Map(_service.SizeAsync)
            .TeeLog(_logger, ComponentName);

    public Either<Error, T[]> Union<T>(string key1, string key2) =>
        (key1, key2)
            .Tee(t => _logger.LogInformation("{Component}: getting union between \"{Key1}\" and \"{Key2}\"", ComponentName, t.key1, t.key2))
            .Map(t => _service.Union<T>(t.key1, t.key2))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, T[]>> UnionAsync<T>(string key1, string key2) =>
        (key1, key2)
            .Tee(t => _logger.LogInformation("{Component}: async getting union between \"{Key1}\" and \"{Key2}\"", ComponentName, t.key1, t.key2))
            .Map(t => _service.UnionAsync<T>(t.key1, t.key2))
            .TeeLog(_logger, ComponentName);
}
