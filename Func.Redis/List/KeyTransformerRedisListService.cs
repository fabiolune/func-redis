namespace Func.Redis.List;

/// <exclude />
internal class KeyTransformerRedisListService(IRedisListService service, Func<string, string> keyMapper) : IRedisListService
{
    private readonly IRedisListService _service = service;
    private readonly Func<string, string> _keyMapper = keyMapper;

    public Either<Error, Unit> Append<T>(string key, T value) =>
        _service.Append(_keyMapper(key), value);

    public Either<Error, Unit> Append<T>(string key, params T[] values) =>
        _service.Append(_keyMapper(key), values);

    public Task<Either<Error, Unit>> AppendAsync<T>(string key, T value) =>
        _service.AppendAsync(_keyMapper(key), value);

    public Task<Either<Error, Unit>> AppendAsync<T>(string key, params T[] values) =>
        _service.AppendAsync(_keyMapper(key), values);

    public Either<Error, Option<T>> Get<T>(string key, long index) =>
        _service.Get<T>(_keyMapper(key), index);

    public Either<Error, Option<T>[]> Get<T>(string key, long start, long stop) =>
        _service.Get<T>(_keyMapper(key), start, stop);

    public Task<Either<Error, Option<T>>> GetAsync<T>(string key, long index) =>
        _service.GetAsync<T>(_keyMapper(key), index);

    public Task<Either<Error, Option<T>[]>> GetAsync<T>(string key, long start, long stop) =>
        _service.GetAsync<T>(_keyMapper(key), start, stop);

    public Either<Error, Option<T>> Pop<T>(string key) =>
        _service.Pop<T>(_keyMapper(key));

    public Either<Error, Option<T>[]> Pop<T>(string key, long count) =>
        _service.Pop<T>(_keyMapper(key), count);

    public Task<Either<Error, Option<T>>> PopAsync<T>(string key) =>
        _service.PopAsync<T>(_keyMapper(key));

    public Task<Either<Error, Option<T>[]>> PopAsync<T>(string key, long count) =>
        _service.PopAsync<T>(_keyMapper(key), count);

    public Either<Error, Unit> Prepend<T>(string key, params T[] values) =>
        _service.Prepend(_keyMapper(key), values);

    public Either<Error, Unit> Prepend<T>(string key, T value) =>
        _service.Prepend(_keyMapper(key), value);

    public Task<Either<Error, Unit>> PrependAsync<T>(string key, params T[] values) =>
        _service.PrependAsync(_keyMapper(key), values);

    public Task<Either<Error, Unit>> PrependAsync<T>(string key, T value) =>
        _service.PrependAsync(_keyMapper(key), value);

    public Either<Error, Option<T>> Shift<T>(string key) =>
        _service.Shift<T>(_keyMapper(key));

    public Either<Error, Option<T>[]> Shift<T>(string key, long count) =>
        _service.Shift<T>(_keyMapper(key), count);

    public Task<Either<Error, Option<T>>> ShiftAsync<T>(string key) =>
        _service.ShiftAsync<T>(_keyMapper(key));

    public Task<Either<Error, Option<T>[]>> ShiftAsync<T>(string key, long count) =>
        _service.ShiftAsync<T>(_keyMapper(key), count);

    public Either<Error, long> Size(string key) =>
        _service.Size(_keyMapper(key));

    public Task<Either<Error, long>> SizeAsync(string key) =>
        _service.SizeAsync(_keyMapper(key));
}
