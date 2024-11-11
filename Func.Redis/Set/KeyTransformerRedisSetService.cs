using TinyFp;

namespace Func.Redis.Set;
public class KeyTransformerRedisSetService(IRedisSetService service, Func<string, string> keyMapper) : IRedisSetService
{
    private readonly Func<string, string> _keyMapper = keyMapper;
    private readonly IRedisSetService _service = service;

    public Either<Error, Unit> Add<T>(string key, T value) => _service.Add(_keyMapper(key), value);
    public Task<Either<Error, Unit>> AddAsync<T>(string key, T value) => _service.AddAsync(_keyMapper(key), value);
    public Either<Error, Unit> Delete<T>(string key, params T[] values) => _service.Delete(_keyMapper(key), values);
    public Either<Error, Unit> Delete<T>(string key, T value) => _service.Delete(_keyMapper(key), value);
    public Task<Either<Error, Unit>> DeleteAsync<T>(string key, params T[] values) => _service.DeleteAsync(_keyMapper(key), values);
    public Task<Either<Error, Unit>> DeleteAsync<T>(string key, T value) => _service.DeleteAsync(_keyMapper(key), value);
    public Either<Error, T[]> Difference<T>(string key1, string key2) => _service.Difference<T>(_keyMapper(key1), _keyMapper(key2));
    public Task<Either<Error, T[]>> DifferenceAsync<T>(string key1, string key2) => _service.DifferenceAsync<T>(_keyMapper(key1), _keyMapper(key2));
    public Either<Error, Option<T>[]> GetAll<T>(string key) => _service.GetAll<T>(_keyMapper(key));
    public Task<Either<Error, Option<T>[]>> GetAllAsync<T>(string key) => _service.GetAllAsync<T>(_keyMapper(key));
    public Either<Error, T[]> Intersect<T>(string key1, string key2) => _service.Intersect<T>(_keyMapper(key1), _keyMapper(key2));
    public Task<Either<Error, T[]>> IntersectAsync<T>(string key1, string key2) => _service.IntersectAsync<T>(_keyMapper(key1), _keyMapper(key2));
    public Either<Error, Option<T>> Pop<T>(string key) => _service.Pop<T>(_keyMapper(key));
    public Task<Either<Error, Option<T>>> PopAsync<T>(string key) => _service.PopAsync<T>(_keyMapper(key));
    public Either<Error, long> Size(string key) => _service.Size(_keyMapper(key));
    public Task<Either<Error, long>> SizeAsync(string key) => _service.SizeAsync(_keyMapper(key));
    public Either<Error, T[]> Union<T>(string key1, string key2) => _service.Union<T>(_keyMapper(key1), _keyMapper(key2));
    public Task<Either<Error, T[]>> UnionAsync<T>(string key1, string key2) => _service.UnionAsync<T>(_keyMapper(key1), _keyMapper(key2));
}
