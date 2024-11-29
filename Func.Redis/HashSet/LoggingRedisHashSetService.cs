using Func.Redis.Extensions;
using Microsoft.Extensions.Logging;
using TinyFp.Extensions;

namespace Func.Redis.HashSet;

public class LoggingRedisHashSetService(ILogger logger,
IRedisHashSetService redisHashSetService) : IRedisHashSetService
{
    private const string NoFieldsWarningTemplate = "{Component}: the key \"{Key}\" contains no fields";

    private readonly ILogger _logger = logger;
    private readonly IRedisHashSetService _redisHashSetService = redisHashSetService;
    private const string ComponentName = nameof(IRedisHashSetService);

    public Either<Error, Unit> Delete(string key, string field) =>
        (key, field)
            .Tee(t => _logger.LogInformation("{Component}: deleting field \"{Field}\" from key \"{Key}\"", ComponentName, t.field, t.key))
            .Map(t => _redisHashSetService.Delete(t.key, t.field))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Delete(string key, params string[] fields) =>
        (key, fields)
            .Tee(t => _logger.LogInformation("{Component}: deleting fields \"{Fields}\" from key \"{Key}\"", ComponentName, t.fields, t.key))
            .Map(t => _redisHashSetService.Delete(t.key, t.fields))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> DeleteAsync(string key, string field) =>
        (key, field)
            .Tee(t => _logger.LogInformation("{Component}: async deleting field \"{Field}\" from key \"{Key}\"", ComponentName, t.field, t.key))
            .Map(t => _redisHashSetService.DeleteAsync(t.key, t.field))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> DeleteAsync(string key, params string[] fields) =>
        (key, fields)
            .Tee(t => _logger.LogInformation("{Component}: async deleting fields \"{Fields}\" from key \"{Key}\"", ComponentName, t.fields, t.key))
            .Map(t => _redisHashSetService.DeleteAsync(t.key, t.fields))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>> Get<T>(string key, string field) =>
        (key, field)
            .Tee(t => _logger.LogInformation("{Component}: getting field \"{Field}\" for key \"{Key}\"", ComponentName, t.field, t.key))
            .Map(t => _redisHashSetService.Get<T>(t.key, t.field))
            .Map(o => o.Tee(n => n.OnNone(() => _logger.LogWarning("{Component}: the key \"{Key}\" does not contain the field \"{Field}\"", ComponentName, key, field))))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>[]> Get<T>(string key, params string[] fields) =>
        (key, fields)
            .Tee(t => _logger.LogInformation("{Component}: getting fields \"{Fields}\" for key \"{Key}\"", ComponentName, t.fields, t.key))
            .Map(t => _redisHashSetService.Get<T>(t.key, t.fields))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<object>[]> Get(string key, params (Type, string)[] typeFields) =>
        (key, typeFields)
            .Tee(t => _logger.LogInformation("{Component}: getting fields \"{Fields}\" for key \"{Key}\"", ComponentName, t.typeFields.Select(tf => tf.Item2), t.key))
            .Map(t => _redisHashSetService.Get(t.key, t.typeFields))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>>> GetAsync<T>(string key, string field) =>
        (key, field)
            .Tee(t => _logger.LogInformation("{Component}: async getting field \"{Field}\" for key \"{Key}\"", ComponentName, t.field, t.key))
            .Map(t => _redisHashSetService.GetAsync<T>(t.key, t.field))
            .MapAsync(o => o.Tee(n => n.OnNone(() => _logger.LogWarning("{Component}: the key \"{Key}\" does not contain the field \"{Field}\"", ComponentName, key, field))))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, params string[] fields) =>
        (key, fields)
            .Tee(t => _logger.LogInformation("{Component}: async getting fields \"{Fields}\" for key \"{Key}\"", ComponentName, t.fields, t.key))
            .Map(t => _redisHashSetService.GetAsync<T>(t.key, t.fields))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<object>[]>> GetAsync(string key, (Type, string)[] typeFields) =>
        (key, typeFields)
            .Tee(t => _logger.LogInformation("{Component}: async getting fields \"{Fields}\" for key \"{Key}\"", ComponentName, t.typeFields.Select(tf => tf.Item2), t.key))
            .Map(t => _redisHashSetService.GetAsync(t.key, t.typeFields))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T[]>> GetValues<T>(string key) =>
        key
           .Tee(k => _logger.LogInformation("{Component}: getting values for key \"{Key}\"", ComponentName, k))
           .Map(_redisHashSetService.GetValues<T>)
           .Map(o => o.Tee(n => n.OnNone(() => _logger.LogWarning(NoFieldsWarningTemplate, ComponentName, key))))
           .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T[]>>> GetValuesAsync<T>(string key) =>
        key
           .Tee(k => _logger.LogInformation("{Component}: async getting values for key \"{Key}\"", ComponentName, k))
           .Map(_redisHashSetService.GetValuesAsync<T>)
           .MapAsync(o => o.Tee(n => n.OnNone(() => _logger.LogWarning(NoFieldsWarningTemplate, ComponentName, key))))
           .TeeLog(_logger, ComponentName);

    public Either<Error, Option<(string, T)[]>> GetAll<T>(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: getting all data for key \"{Key}\"", ComponentName, k))
            .Map(_redisHashSetService.GetAll<T>)
            .Map(o => o.Tee(n => n.OnNone(() => _logger.LogWarning(NoFieldsWarningTemplate, ComponentName, key))))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<(string, T)[]>>> GetAllAsync<T>(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: async getting all data for key \"{Key}\"", ComponentName, k))
            .Map(_redisHashSetService.GetAllAsync<T>)
            .MapAsync(o => o.Tee(n => n.OnNone(() => _logger.LogWarning(NoFieldsWarningTemplate, ComponentName, key))))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Set<T>(string key, string field, T value) =>
        (key, field, value)
            .Tee(t => _logger.LogInformation("{Component}: setting field \"{Field}\" for key \"{Key}\"", ComponentName, t.field, t.key))
            .Map(t => _redisHashSetService.Set(t.key, t.field, t.value))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Set<T>(string key, params (string, T)[] pairs) =>
        (key, pairs)
            .Tee(t => _logger.LogInformation("{Component}: setting fields \"{Fields}\" for key \"{Key}\"", ComponentName, t.pairs.Select(p => p.Item1).ToArray(), t.key))
            .Map(t => _redisHashSetService.Set(t.key, t.pairs))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> SetAsync<T>(string key, string field, T value) =>
        (key, field, value)
            .Tee(t => _logger.LogInformation("{Component}: async setting field \"{Field}\" for key \"{Key}\"", ComponentName, t.field, t.key))
            .Map(t => _redisHashSetService.SetAsync(t.key, t.field, t.value))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> SetAsync<T>(string key, params (string, T)[] pairs) =>
        (key, pairs)
            .Tee(t => _logger.LogInformation("{Component}: async setting fields \"{Fields}\" for key \"{Key}\"", ComponentName, t.pairs.Select(p => p.Item1).ToArray(), t.key))
            .Map(t => _redisHashSetService.SetAsync(t.key, t.pairs))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<string[]>> GetFieldKeys(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: getting field keys for key \"{Key}\"", ComponentName, k))
            .Map(_redisHashSetService.GetFieldKeys)
            .Map(o => o.Tee(n => n.OnNone(() => _logger.LogWarning(NoFieldsWarningTemplate, ComponentName, key))))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<string[]>>> GetFieldKeysAsync(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: async getting field keys for key \"{Key}\"", ComponentName, k))
            .Map(_redisHashSetService.GetFieldKeysAsync)
            .MapAsync(o => o.Tee(n => n.OnNone(() => _logger.LogWarning(NoFieldsWarningTemplate, ComponentName, key))))
            .TeeLog(_logger, ComponentName);
}