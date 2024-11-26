using Func.Redis.Extensions;
using Microsoft.Extensions.Logging;

namespace Func.Redis.List;
internal class LoggingRedisListService(ILogger logger, IRedisListService service) : IRedisListService
{
    private readonly IRedisListService _service = service;
    private readonly ILogger _logger = logger;
    private const string ComponentName = nameof(IRedisListService);

    public Either<Error, Unit> Append<T>(string key, T value) =>
        _service.Append(key, value).TeeLog(_logger, ComponentName);
    
    public Either<Error, Unit> Append<T>(string key, params T[] values) =>
        _service.Append(key, values).TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> AppendAsync<T>(string key, T value) =>
        _service.AppendAsync(key, value).TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> AppendAsync<T>(string key, params T[] values) =>
        _service.AppendAsync(key, values).TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>> Get<T>(string key, long index) =>
        _service.Get<T>(key, index).TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>[]> Get<T>(string key, long start, long stop) =>
        _service.Get<T>(key, start, stop).TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>>> GetAsync<T>(string key, long index) =>
        _service.GetAsync<T>(key, index).TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, long start, long stop) =>
        _service.GetAsync<T>(key, start, stop).TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>> Pop<T>(string key) =>
        _service.Pop<T>(key).TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>[]> Pop<T>(string key, long count) =>
        _service.Pop<T>(key, count).TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>>> PopAsync<T>(string key) =>
        _service.PopAsync<T>(key).TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>[]>> PopAsync<T>(string key, long count) =>
        _service.PopAsync<T>(key, count).TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Prepend<T>(string key, params T[] values) =>
        _service.Prepend(key, values).TeeLog(_logger, ComponentName);

    public Either<Error, Unit> Prepend<T>(string key, T value) =>
        _service.Prepend(key, value).TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> PrependAsync<T>(string key, params T[] values) =>
        _service.PrependAsync(key, values).TeeLog(_logger, ComponentName);

    public Task<Either<Error, Unit>> PrependAsync<T>(string key, T value) =>
        _service.PrependAsync(key, value).TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>> Shift<T>(string key) =>
        _service.Shift<T>(key).TeeLog(_logger, ComponentName);

    public Either<Error, Option<T>[]> Shift<T>(string key, long count) =>
        _service.Shift<T>(key, count).TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>>> ShiftAsync<T>(string key) =>
        _service.ShiftAsync<T>(key).TeeLog(_logger, ComponentName);

    public Task<Either<Error, Option<T>[]>> ShiftAsync<T>(string key, long count) =>
        _service.ShiftAsync<T>(key, count).TeeLog(_logger, ComponentName);

    public Either<Error, long> Size(string key) =>
        _service.Size(key).TeeLog(_logger, ComponentName);

    public Task<Either<Error, long>> SizeAsync(string key) =>
        _service.SizeAsync(key).TeeLog(_logger, ComponentName);
}
