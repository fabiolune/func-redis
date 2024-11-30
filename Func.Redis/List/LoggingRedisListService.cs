using Func.Redis.Extensions;
using Microsoft.Extensions.Logging;
using TinyFp.Extensions;

namespace Func.Redis.List;
internal class LoggingRedisListService(
    ILogger<IRedisListService> logger,
    IRedisListService service) : IRedisListService
{
    private readonly IRedisListService _service = service;
    private readonly ILogger _logger = logger;
    private const string ComponentName = nameof(IRedisListService);

    public Either<Error, Unit> Append<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: appending value to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.Append(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Append<T>(string key, params T[] values) =>
        (key, values)
            .Tee(t => _logger.LogInformation("{Component}: appending values to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.Append(t.key, t.values))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> AppendAsync<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: async appending value to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.AppendAsync(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> AppendAsync<T>(string key, params T[] values) =>
        (key, values)
            .Tee(t => _logger.LogInformation("{Component}: async appending values to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.AppendAsync(t.key, t.values))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>> Get<T>(string key, long index) =>
        (key, index)
            .Tee(t => _logger.LogInformation("{Component}: getting value at \"{Key}\" for index \"{Index}\"", ComponentName, t.key, t.index))
            .Map(t => _service.Get<T>(t.key, t.index))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>[]> Get<T>(string key, long start, long stop) =>
        (key, start, stop)
            .Tee(t => _logger.LogInformation("{Component}: getting values at \"{Key}\" between \"{Start}\" and \"{Stop}\"", ComponentName, t.key, t.start, t.stop))
            .Map(t => _service.Get<T>(t.key, t.start, t.stop))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>>> GetAsync<T>(string key, long index) =>
        (key, index)
            .Tee(t => _logger.LogInformation("{Component}: async getting value at \"{Key}\" for index \"{Index}\"", ComponentName, t.key, t.index))
            .Map(t => _service.GetAsync<T>(t.key, t.index))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, long start, long stop) =>
        (key, start, stop)
            .Tee(t => _logger.LogInformation("{Component}: async getting values at \"{Key}\" between \"{Start}\" and \"{Stop}\"", ComponentName, t.key, t.start, t.stop))
            .Map(t => _service.GetAsync<T>(t.key, t.start, t.stop))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>> Pop<T>(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: popping item from \"{Key}\"", ComponentName, k))
            .Map(_service.Pop<T>)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>[]> Pop<T>(string key, long count) =>
        (key, count)
            .Tee(t => _logger.LogInformation("{Component}: popping \"{Count}\" items from \"{Key}\"", ComponentName, t.count, t.key))
            .Map(t => _service.Pop<T>(t.key, t.count))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>>> PopAsync<T>(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: async popping item from \"{Key}\"", ComponentName, k))
            .Map(_service.PopAsync<T>)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>[]>> PopAsync<T>(string key, long count) =>
        (key, count)
            .Tee(t => _logger.LogInformation("{Component}: async popping \"{Count}\" items from \"{Key}\"", ComponentName, t.count, t.key))
            .Map(t => _service.PopAsync<T>(t.key, t.count))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Prepend<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: prepending value to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.Prepend(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Prepend<T>(string key, params T[] values) =>
        (key, values)
            .Tee(t => _logger.LogInformation("{Component}: prepending values to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.Prepend(t.key, t.values))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> PrependAsync<T>(string key, T value) =>
        (key, value)
            .Tee(t => _logger.LogInformation("{Component}: async prepending value to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.PrependAsync(t.key, t.value))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> PrependAsync<T>(string key, params T[] values) =>
        (key, values)
            .Tee(t => _logger.LogInformation("{Component}: async prepending values to \"{Key}\"", ComponentName, t.key))
            .Map(t => _service.PrependAsync(t.key, t.values))
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>> Shift<T>(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: shifting item from \"{Key}\"", ComponentName, k))
            .Map(_service.Shift<T>)
            .TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>[]> Shift<T>(string key, long count) =>
        (key, count)
            .Tee(t => _logger.LogInformation("{Component}: shifting \"{Count}\" items from \"{Key}\"", ComponentName, t.count, t.key))
            .Map(t => _service.Shift<T>(t.key, t.count))
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>>> ShiftAsync<T>(string key) =>
        key
            .Tee(k => _logger.LogInformation("{Component}: async shifting item from \"{Key}\"", ComponentName, k))
            .Map(_service.ShiftAsync<T>)
            .TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>[]>> ShiftAsync<T>(string key, long count) =>
        (key, count)
            .Tee(t => _logger.LogInformation("{Component}: async shifting \"{Count}\" items from \"{Key}\"", ComponentName, t.count, t.key))
            .Map(t => _service.ShiftAsync<T>(t.key, t.count))
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
}
