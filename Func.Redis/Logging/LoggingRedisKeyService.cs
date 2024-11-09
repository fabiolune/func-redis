using Func.Redis.Extensions;
using Microsoft.Extensions.Logging;
using TinyFp;
using TinyFp.Extensions;

namespace Func.Redis.Logging;

public class LoggingRedisKeyService(ILogger logger,
    IRedisKeyService redisService) : IRedisKeyService
{
    private readonly ILogger _logger = logger;
    private readonly IRedisKeyService _redisService = redisService;
    private const string ComponentName = nameof(IRedisKeyService);

    public Either<Error, Unit> Delete(string key) =>
        _redisService
            .Delete(key)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Delete(params string[] keys) =>
        _redisService
            .Delete(keys)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> DeleteAsync(string key) =>
        _redisService
            .DeleteAsync(key)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> DeleteAsync(params string[] keys) =>
        _redisService
            .DeleteAsync(keys)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>> Get<T>(string key) =>
        _redisService
            .Get<T>(key)
            .Map(opt => opt.Tee(o => o.OnNone(() => _logger.LogWarning("{Component}: key \"{Key}\" not found", ComponentName, key))))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>[]> Get<T>(params string[] keys) =>
        _redisService
            .Get<T>(keys)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>>> GetAsync<T>(string key) =>
        _redisService
            .GetAsync<T>(key)
            .MapAsync(opt => opt.Tee(o => o.OnNone(() => _logger.LogWarning("{Component}: key \"{Key}\" not found", ComponentName, key))))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>[]>> GetAsync<T>(params string[] keys) =>
        _redisService
            .GetAsync<T>(keys)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Set<T>(string key, T value) =>
        _redisService
            .Set(key, value)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Set<T>(params (string, T)[] pairs) =>
        _redisService
            .Set(pairs)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> SetAsync<T>(string key, T value) =>
        _redisService
            .SetAsync(key, value)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> SetAsync<T>(params (string, T)[] pairs) =>
        _redisService
            .SetAsync(pairs)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> RenameKey(string key, string newKey) =>
        _redisService
            .RenameKey(key, newKey)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> RenameKeyAsync(string key, string newKey) =>
        _redisService
            .RenameKeyAsync(key, newKey)
            .TeeLog(_logger, ComponentName);

    public Either<Error, string[]> GetKeys(string pattern) =>
        _redisService
            .GetKeys(pattern)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, string[]>> GetKeysAsync(string pattern) =>
        _redisService
            .GetKeysAsync(pattern)
            .TeeLog(_logger, ComponentName);
}