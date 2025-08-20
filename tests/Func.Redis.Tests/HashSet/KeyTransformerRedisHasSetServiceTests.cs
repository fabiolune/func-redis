using Func.Redis.HashSet;

namespace Func.Redis.Tests.HashSet;
internal class KeyTransformerRedisHasSetServiceTests
{
    private IRedisHashSetService _mockService;
    private KeyTransformerRedisHashSetService _sut;

    [SetUp]
    public void SetUp()
    {
        _mockService = Substitute.For<IRedisHashSetService>();
        _sut = new KeyTransformerRedisHashSetService(_mockService, k => $"mapped_{k}");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void Delete_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.Delete("mapped_key", "field").Returns(result);

        var actual = _sut.Delete("key", "field");

        actual.ShouldBe(result);
        _mockService.Received(1).Delete("mapped_key", "field");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task DeleteAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.DeleteAsync("mapped_key", "field").Returns(result);

        var actual = await _sut.DeleteAsync("key", "field");

        actual.ShouldBe(result);
        await _mockService.Received(1).DeleteAsync("mapped_key", "field");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void DeleteMultiple_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.Delete("mapped_key", "field1", "field2").Returns(result);

        var actual = _sut.Delete("key", "field1", "field2");

        actual.ShouldBe(result);
        _mockService.Received(1).Delete("mapped_key", "field1", "field2");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task DeleteMultipleAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.DeleteAsync("mapped_key", "field1", "field2").Returns(result);

        var actual = await _sut.DeleteAsync("key", "field1", "field2");

        actual.ShouldBe(result);
        await _mockService.Received(1).DeleteAsync("mapped_key", "field1", "field2");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringTestData))]
    public void Get_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> result)
    {
        _mockService.Get<string>("mapped_key", "field").Returns(result);

        var actual = _sut.Get<string>("key", "field");

        actual.ShouldBe(result);
        _mockService.Received(1).Get<string>("mapped_key", "field");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringTestData))]
    public async Task GetAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> result)
    {
        _mockService.GetAsync<string>("mapped_key", "field").Returns(result);

        var actual = await _sut.GetAsync<string>("key", "field");

        actual.ShouldBe(result);
        await _mockService.Received(1).GetAsync<string>("mapped_key", "field");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorOptionStringsTestData))]
    public void GetMultiple_ShouldCallServiceWithMappedKey(Either<Error, Option<string>[]> result)
    {
        _mockService.Get<string>("mapped_key", "field1", "field2").Returns(result);

        var actual = _sut.Get<string>("key", "field1", "field2");

        actual.ShouldBe(result);
        _mockService.Received(1).Get<string>("mapped_key", "field1", "field2");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorOptionStringsTestData))]
    public async Task GetMultipleAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<string>[]> result)
    {
        _mockService.GetAsync<string>("mapped_key", "field1", "field2").Returns(result);

        var actual = await _sut.GetAsync<string>("key", "field1", "field2");

        actual.ShouldBe(result);
        await _mockService.Received(1).GetAsync<string>("mapped_key", "field1", "field2");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorObjectsTestData))]
    public void GetWithType_ShouldCallServiceWithMappedKey(Either<Error, Option<object>[]> result)
    {
        _mockService.Get("mapped_key", (typeof(TestData), "field1"), (typeof(TestData), "field2")).Returns(result);

        var actual = _sut.Get("key", (typeof(TestData), "field1"), (typeof(TestData), "field2"));

        actual.ShouldBe(result);
        _mockService.Received(1).Get("mapped_key", (typeof(TestData), "field1"), (typeof(TestData), "field2"));
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorObjectsTestData))]
    public async Task GetWithTypeAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<object>[]> result)
    {
        _mockService.GetAsync("mapped_key", (typeof(TestData), "field1"), (typeof(TestData), "field2")).Returns(result);

        var actual = await _sut.GetAsync("key", (typeof(TestData), "field1"), (typeof(TestData), "field2"));

        actual.ShouldBe(result);
        await _mockService.Received(1).GetAsync("mapped_key", (typeof(TestData), "field1"), (typeof(TestData), "field2"));
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void Set_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.Set("mapped_key", "field", Arg.Any<TestData>()).Returns(result);

        var actual = _sut.Set("key", "field", new TestData(1));

        actual.ShouldBe(result);
        _mockService.Received(1).Set("mapped_key", "field", Arg.Any<TestData>());
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task SetAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.SetAsync("mapped_key", "field", Arg.Any<TestData>()).Returns(result);

        var actual = await _sut.SetAsync("key", "field", new TestData(1));

        actual.ShouldBe(result);
        await _mockService.Received(1).SetAsync("mapped_key", "field", Arg.Any<TestData>());
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void SetMultiple_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.Set("mapped_key", ("field1", new TestData(1)), ("field2", new TestData(1))).Returns(result);

        var actual = _sut.Set("key", ("field1", new TestData(1)), ("field2", new TestData(1)));

        actual.ShouldBe(result);
        _mockService.Received(1).Set("mapped_key", ("field1", new TestData(1)), ("field2", new TestData(1)));
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task SetMultipleAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.SetAsync("mapped_key", ("field1", new TestData(1)), ("field2", new TestData(1))).Returns(result);

        var actual = await _sut.SetAsync("key", ("field1", new TestData(1)), ("field2", new TestData(1)));

        actual.ShouldBe(result);
        await _mockService.Received(1).SetAsync("mapped_key", ("field1", new TestData(1)), ("field2", new TestData(1)));
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorTuplesTestData))]
    public void GetAll_ShouldCallServiceWithMappedKey(Either<Error, Option<(string, TestData)[]>> result)
    {
        _mockService.GetAll<TestData>("mapped_key").Returns(result);

        var actual = _sut.GetAll<TestData>("key");

        actual.ShouldBe(result);
        _mockService.Received(1).GetAll<TestData>("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorTuplesTestData))]
    public async Task GetAllAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<(string, TestData)[]>> result)
    {
        _mockService.GetAllAsync<TestData>("mapped_key").Returns(result);

        var actual = await _sut.GetAllAsync<TestData>("key");

        actual.ShouldBe(result);
        await _mockService.Received(1).GetAllAsync<TestData>("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorTestDataTestData))]
    public void GetValues_ShouldCallServiceWithMappedKey(Either<Error, Option<TestData[]>> result)
    {
        _mockService.GetValues<TestData>("mapped_key").Returns(result);

        var actual = _sut.GetValues<TestData>("key");

        actual.ShouldBe(result);
        _mockService.Received(1).GetValues<TestData>("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorTestDataTestData))]
    public async Task GetValuesAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<TestData[]>> result)
    {
        _mockService.GetValuesAsync<TestData>("mapped_key").Returns(result);

        var actual = await _sut.GetValuesAsync<TestData>("key");

        actual.ShouldBe(result);
        await _mockService.Received(1).GetValuesAsync<TestData>("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringArrayTestData))]
    public void GetFieldKeys_ShouldCallServiceWithMappedKey(Either<Error, Option<string[]>> result)
    {
        _mockService.GetFieldKeys("mapped_key").Returns(result);

        var actual = _sut.GetFieldKeys("key");

        actual.ShouldBe(result);
        _mockService.Received(1).GetFieldKeys("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringArrayTestData))]
    public async Task GetFieldKeysAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<string[]>> result)
    {
        _mockService.GetFieldKeysAsync("mapped_key").Returns(result);

        var actual = await _sut.GetFieldKeysAsync("key");

        actual.ShouldBe(result);
        await _mockService.Received(1).GetFieldKeysAsync("mapped_key");
    }
}
