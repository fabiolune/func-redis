using Func.Redis.Extensions;
using Microsoft.Extensions.Logging;
using TinyFp.Extensions;

namespace Func.Redis.Key;

public class LoggingRedisKeyService(ILogger logger,
    IRedisKeyService redisService) : IRedisKeyService
{
    private readonly ILogger _logger = logger;
    private readonly IRedisKeyService _redisService = redisService;
    private const string ComponentName = nameof(IRedisKeyService);

    public Either<Error, Unit> Delete(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: deleting key \"{Key}\"", ComponentName, k))
            .Map(_redisService.Delete)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Delete(params string[] keys) =>
        keys
            .Tee(k => _logger.LogInformation("{Component}: deleting keys \"{Keys}\"", ComponentName, k))
            .Map(_redisService.Delete)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> DeleteAsync(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: async deleting key \"{Key}\"", ComponentName, k))
            .Map(_redisService.DeleteAsync)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> DeleteAsync(params string[] keys) =>
        keys
            .Tee(k => _logger.LogInformation("{Component}: async deleting keys \"{Keys}\"", ComponentName, k))
            .Map(_redisService.DeleteAsync)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>> Get<T>(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: getting key \"{Key}\"", ComponentName, k))
            .Map(_redisService.Get<T>)
            .Map(opt => opt.Tee(o => o.OnNone(() => _logger.LogWarning("{Component}: key \"{Key}\" not found", ComponentName, key))))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>[]> Get<T>(params string[] keys) =>
        keys
            .Tee(k => _logger.LogInformation("{Component}: getting keys \"{Keys}\"", ComponentName, k))
            .Map(_redisService.Get<T>)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>>> GetAsync<T>(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: async getting key \"{Key}\"", ComponentName, k))
            .Map(_redisService.GetAsync<T>)
            .MapAsync(opt => opt.Tee(o => o.OnNone(() => _logger.LogWarning("{Component}: key \"{Key}\" not found", ComponentName, key))))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>[]>> GetAsync<T>(params string[] keys) =>
        keys
            .Tee(k => _logger.LogInformation("{Component}: async getting keys \"{Keys}\"", ComponentName, k))
            .Map(_redisService.GetAsync<T>)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Set<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: setting key \"{Key}\"", ComponentName, t.key))
            .Map(t => _redisService.Set(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Set<T>(params (string, T)[] pairs) =>
        pairs
            .Tee(ps => _logger.LogInformation("{Component}: setting keys \"{Keys}\"", ComponentName, ps.Select(p => p.Item1).ToArray()))
            .Map(_redisService.Set)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> SetAsync<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: async setting key \"{Key}\"", ComponentName, t.key))
            .Map(t => _redisService.SetAsync(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> SetAsync<T>(params (string, T)[] pairs) =>
        pairs
            .Tee(ps => _logger.LogInformation("{Component}: async setting keys \"{Keys}\"", ComponentName, ps.Select(p => p.Item1).ToArray()))
            .Map(_redisService.SetAsync)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> RenameKey(string key, string newKey) =>
        (key, newKey)
            .Tee(t => _logger.LogInformation("{Component}: renaming key \"{Key}\" to \"{NewKey}\"", ComponentName, t.key, t.newKey))
            .Map(t => _redisService.RenameKey(t.key, t.newKey))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> RenameKeyAsync(string key, string newKey) =>
        (key, newKey)
            .Tee(t => _logger.LogInformation("{Component}: async renaming key \"{Key}\" to \"{NewKey}\"", ComponentName, t.key, t.newKey))
            .Map(t => _redisService.RenameKeyAsync(t.key, t.newKey))
            .TeeLog(_logger, ComponentName);

    public Either<Error, string[]> GetKeys(string pattern) =>
        pattern
            .Tee(p => _logger.LogInformation("{Component}: getting keys with pattern \"{Pattern}\"", ComponentName, p))
            .Map(_redisService.GetKeys)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, string[]>> GetKeysAsync(string pattern) =>
        pattern
            .Tee(p => _logger.LogInformation("{Component}: async getting keys with pattern \"{Pattern}\"", ComponentName, p))
            .Map(_redisService.GetKeysAsync)
            .TeeLog(_logger, ComponentName);
}