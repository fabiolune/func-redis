using Func.Redis.Extensions;
using Microsoft.Extensions.Logging;
using TinyFp;
using TinyFp.Extensions;

namespace Func.Redis;

public class LoggingRedisHashSetService(ILogger logger,
IRedisHashSetService redisHashSetService) : IRedisHashSetService
{
    private const string NoFieldsWarningTemplate = "{Component}: the key \"{Key}\" contains no fields";

    private readonly ILogger _logger = logger;
    private readonly IRedisHashSetService _redisHashSetService = redisHashSetService;
    private const string ComponentName = nameof(IRedisHashSetService);

    public Either<Error, Unit> Delete(string key, string field) =>
        _redisHashSetService
            .Delete(key, field)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Delete(string key, params string[] fields) =>
        _redisHashSetService
            .Delete(key, fields)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> DeleteAsync(string key, string field) =>
        _redisHashSetService
            .DeleteAsync(key, field)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> DeleteAsync(string key, params string[] fields) =>
        _redisHashSetService
            .DeleteAsync(key, fields)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>> Get<T>(string key, string field) =>
        _redisHashSetService
            .Get<T>(key, field)
            .Map(o => o.Tee(n => n.OnNone(() => _logger.LogWarning("{Component}: the key \"{Key}\" does not contain the field \"{Field}\"", ComponentName, key, field))))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>[]> Get<T>(string key, params string[] fields) =>
        _redisHashSetService
            .Get<T>(key, fields)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<object>[]> Get(string key, params (Type, string)[] typeFields) =>
        _redisHashSetService
            .Get(key, typeFields)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>>> GetAsync<T>(string key, string field) =>
        _redisHashSetService
            .GetAsync<T>(key, field)
            .MapAsync(o => o.Tee(n => n.OnNone(() => _logger.LogWarning("{Component}: the key \"{Key}\" does not contain the field \"{Field}\"", ComponentName, key, field))))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, params string[] fields) =>
        _redisHashSetService
            .GetAsync<T>(key, fields)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<object>[]>> GetAsync(string key, (Type, string)[] typeFields) =>
         _redisHashSetService
             .GetAsync(key, typeFields)
             .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T[]>> GetValues<T>(string key) =>
        _redisHashSetService
            .GetValues<T>(key)
            .Map(o => o.Tee(n => n.OnNone(() => _logger.LogWarning(NoFieldsWarningTemplate, ComponentName, key))))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T[]>>> GetValuesAsync<T>(string key) =>
        _redisHashSetService
            .GetValuesAsync<T>(key)
            .MapAsync(o => o.Tee(n => n.OnNone(() => _logger.LogWarning(NoFieldsWarningTemplate, ComponentName, key))))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<(string, T)[]>> GetAll<T>(string key)
        => _redisHashSetService
            .GetAll<T>(key)
            .Map(o => o.Tee(n => n.OnNone(() => _logger.LogWarning(NoFieldsWarningTemplate, ComponentName, key))))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<(string, T)[]>>> GetAllAsync<T>(string key)
        => _redisHashSetService
            .GetAllAsync<T>(key)
            .MapAsync(o => o.Tee(n => n.OnNone(() => _logger.LogWarning(NoFieldsWarningTemplate, ComponentName, key))))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Set<T>(string key, string field, T value) =>
        _redisHashSetService
            .Set(key, field, value)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Set<T>(string key, params (string, T)[] pairs) =>
        _redisHashSetService
            .Set(key, pairs)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> SetAsync<T>(string key, string field, T value) =>
        _redisHashSetService
            .SetAsync(key, field, value)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> SetAsync<T>(string key, params (string, T)[] pairs) =>
        _redisHashSetService
            .SetAsync(key, pairs)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<string[]>> GetFieldKeys(string key) =>
        _redisHashSetService
            .GetFieldKeys(key)
            .Map(o => o.Tee(n => n.OnNone(() => _logger.LogWarning(NoFieldsWarningTemplate, ComponentName, key))))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<string[]>>> GetFieldKeysAsync(string key) =>
         _redisHashSetService
            .GetFieldKeysAsync(key)
            .MapAsync(o => o.Tee(n => n.OnNone(() => _logger.LogWarning(NoFieldsWarningTemplate, ComponentName, key))))
            .TeeLog(_logger, ComponentName);
}