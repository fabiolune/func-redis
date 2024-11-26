using Func.Redis.Set;
using static Func.Redis.Tests.TestDataElements;

namespace Func.Redis.Tests.Set;
internal class KeyTransformerRedisSetServiceTests
{
    private KeyTransformerRedisSetService _sut;
    private IRedisSetService _mockService;

    [SetUp]
    public void SetUp()
    {
        _mockService = Substitute.For<IRedisSetService>();
        _sut = new KeyTransformerRedisSetService(_mockService, k => $"mapped_{k}");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void Add_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.Add("mapped_key", "value").Returns(internalResult);

        var result = _sut.Add(key, "value");

        result.Should().Be(internalResult);
        _mockService.Received(1).Add("mapped_key", "value");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task AddAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.AddAsync("mapped_key", "value").Returns(internalResult);

        var result = await _sut.AddAsync(key, "value");

        result.Should().Be(internalResult);
        await _mockService.Received(1).AddAsync("mapped_key", "value");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void Delete_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.Delete("mapped_key", "value").Returns(internalResult);

        var result = _sut.Delete(key, "value");

        result.Should().Be(internalResult);
        _mockService.Received(1).Delete("mapped_key", "value");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void DeleteMultiple_ShouldCallServiceWithMappedKeys(Either<Error, Unit> internalResult)
    {
        _mockService.Delete("mapped_key", "value1", "value2").Returns(internalResult);

        var result = _sut.Delete("key", "value1", "value2");

        result.Should().Be(internalResult);
        _mockService.Received(1).Delete("mapped_key", "value1", "value2");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task DeleteAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.DeleteAsync("mapped_key", "value").Returns(internalResult);

        var result = await _sut.DeleteAsync(key, "value");

        result.Should().Be(internalResult);
        await _mockService.Received(1).DeleteAsync("mapped_key", "value");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task DeleteMultipleAsync_ShouldCallServiceWithMappedKeys(Either<Error, Unit> internalResult)
    {
        _mockService.DeleteAsync("mapped_key", "value1", "value2").Returns(internalResult);

        var result = await _sut.DeleteAsync("key", "value1", "value2");

        result.Should().Be(internalResult);
        await _mockService.Received(1).DeleteAsync("mapped_key", "value1", "value2");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorOptionStringsTestData))]
    public void GetAll_ShouldCallServiceWithMappedKey(Either<Error, Option<string>[]> internalResult)
    {
        _mockService.GetAll<string>("mapped_key").Returns(internalResult);

        var result = _sut.GetAll<string>("key");

        result.Should().Be(internalResult);
        _mockService.Received(1).GetAll<string>("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorOptionStringsTestData))]
    public async Task GetAllAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<string>[]> internalResult)
    {
        _mockService.GetAllAsync<string>("mapped_key").Returns(internalResult);

        var result = await _sut.GetAllAsync<string>("key");

        result.Should().Be(internalResult);
        await _mockService.Received(1).GetAllAsync<string>("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringsTestData))]
    public void Difference_ShouldCallServiceWithMappedKeys(Either<Error, string[]> internalResult)
    {
        _mockService.Difference<string>("mapped_key1", "mapped_key2").Returns(internalResult);

        var result = _sut.Difference<string>("key1", "key2");

        result.Should().Be(internalResult);
        _mockService.Received(1).Difference<string>("mapped_key1", "mapped_key2");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringsTestData))]
    public async Task DifferenceAsync_ShouldCallServiceWithMappedKeys(Either<Error, string[]> internalResult)
    {
        _mockService.DifferenceAsync<string>("mapped_key1", "mapped_key2").Returns(internalResult);

        var result = await _sut.DifferenceAsync<string>("key1", "key2");

        result.Should().Be(internalResult);
        await _mockService.Received(1).DifferenceAsync<string>("mapped_key1", "mapped_key2");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringsTestData))]
    public void Intersect_ShouldCallServiceWithMappedKeys(Either<Error, string[]> internalResult)
    {
        _mockService.Intersect<string>("mapped_key1", "mapped_key2").Returns(internalResult);

        var result = _sut.Intersect<string>("key1", "key2");

        result.Should().Be(internalResult);
        _mockService.Received(1).Intersect<string>("mapped_key1", "mapped_key2");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringsTestData))]
    public async Task IntersectAsync_ShouldCallServiceWithMappedKeys(Either<Error, string[]> internalResult)
    {
        _mockService.IntersectAsync<string>("mapped_key1", "mapped_key2").Returns(internalResult);

        var result = await _sut.IntersectAsync<string>("key1", "key2");

        result.Should().Be(internalResult);
        await _mockService.Received(1).IntersectAsync<string>("mapped_key1", "mapped_key2");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringsTestData))]
    public void Union_ShouldCallServiceWithMappedKeys(Either<Error, string[]> internalResult)
    {
        _mockService.Union<string>("mapped_key1", "mapped_key2").Returns(internalResult);

        var result = _sut.Union<string>("key1", "key2");

        result.Should().Be(internalResult);
        _mockService.Received(1).Union<string>("mapped_key1", "mapped_key2");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringsTestData))]
    public async Task UnionAsync_ShouldCallServiceWithMappedKeys(Either<Error, string[]> internalResult)
    {
        _mockService.UnionAsync<string>("mapped_key1", "mapped_key2").Returns(internalResult);

        var result = await _sut.UnionAsync<string>("key1", "key2");

        result.Should().Be(internalResult);
        await _mockService.Received(1).UnionAsync<string>("mapped_key1", "mapped_key2");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringTestData))]
    public void Pop_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> internalResult)
    {
        _mockService.Pop<string>("mapped_key").Returns(internalResult);

        var result = _sut.Pop<string>("key");

        result.Should().Be(internalResult);
        _mockService.Received(1).Pop<string>("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringTestData))]
    public async Task PopAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> internalResult)
    {
        _mockService.PopAsync<string>("mapped_key").Returns(internalResult);

        var result = await _sut.PopAsync<string>("key");

        result.Should().Be(internalResult);
        await _mockService.Received(1).PopAsync<string>("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorLongTestData))]
    public void Size_ShouldCallServiceWithMappedKey(Either<Error, long> internalResult)
    {
        _mockService.Size("mapped_key").Returns(internalResult);

        var result = _sut.Size("key");

        result.Should().Be(internalResult);
        _mockService.Received(1).Size("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorLongTestData))]
    public async Task SizeAsync_ShouldCallServiceWithMappedKey(Either<Error, long> internalResult)
    {
        _mockService.SizeAsync("mapped_key").Returns(internalResult);

        var result = await _sut.SizeAsync("key");

        result.Should().Be(internalResult);
        await _mockService.Received(1).SizeAsync("mapped_key");
    }
}