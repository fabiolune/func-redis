namespace Func.Redis.Tests;
internal class KeyTransformerRedisHasSetServiceTests
{
    private IRedisHashSetService _mockService;
    private KeyTransformerRedisHasSetService _sut;

    internal record TestData
    {
        public int Id { get; init; }
    }

    [SetUp]
    public void SetUp()
    {
        _mockService = Substitute.For<IRedisHashSetService>();
        _sut = new KeyTransformerRedisHasSetService(_mockService, k => $"mapped_{k}");
    }

    private static readonly Either<Error, Unit> RightErrorUnit = Either<Error, Unit>.Right(Unit.Default);
    private static readonly Either<Error, Unit> LeftErrorUnit = Either<Error, Unit>.Left(Error.New("error"));
    public static readonly Either<Error, Unit>[] ErrorUnitTestData = [RightErrorUnit, LeftErrorUnit];

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public void Delete_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.Delete("mapped_key", "field").Returns(result);

        var actual = _sut.Delete("key", "field");

        actual.Should().Be(result);
        _mockService.Received(1).Delete("mapped_key", "field");
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public async Task DeleteAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.DeleteAsync("mapped_key", "field").Returns(result);

        var actual = await _sut.DeleteAsync("key", "field");

        actual.Should().Be(result);
        await _mockService.Received(1).DeleteAsync("mapped_key", "field");
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public void DeleteMultiple_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.Delete("mapped_key", "field1", "field2").Returns(result);

        var actual = _sut.Delete("key", "field1", "field2");

        actual.Should().Be(result);
        _mockService.Received(1).Delete("mapped_key", "field1", "field2");
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public async Task DeleteMultipleAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.DeleteAsync("mapped_key", "field1", "field2").Returns(result);

        var actual = await _sut.DeleteAsync("key", "field1", "field2");

        actual.Should().Be(result);
        await _mockService.Received(1).DeleteAsync("mapped_key", "field1", "field2");
    }

    private static readonly Either<Error, Option<string>> SomeErrorOptionString = Either<Error, Option<string>>.Right(Option<string>.Some("success"));
    private static readonly Either<Error, Option<string>> NoneErrorOptionString = Either<Error, Option<string>>.Right(Option<string>.None());
    private static readonly Either<Error, Option<string>> LeftErrorOptionString = Either<Error, Option<string>>.Left(Error.New("error"));
    public static readonly Either<Error, Option<string>>[] ErrorStringTestData = [SomeErrorOptionString, NoneErrorOptionString, LeftErrorOptionString];

    [TestCaseSource(nameof(ErrorStringTestData))]
    public void Get_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> result)
    {
        _mockService.Get<string>("mapped_key", "field").Returns(result);

        var actual = _sut.Get<string>("key", "field");

        actual.Should().Be(result);
        _mockService.Received(1).Get<string>("mapped_key", "field");
    }

    [TestCaseSource(nameof(ErrorStringTestData))]
    public async Task GetAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> result)
    {
        _mockService.GetAsync<string>("mapped_key", "field").Returns(result);

        var actual = await _sut.GetAsync<string>("key", "field");

        actual.Should().Be(result);
        await _mockService.Received(1).GetAsync<string>("mapped_key", "field");
    }

    private static readonly Either<Error, Option<string>[]> SomeErrorOptionStrings = Either<Error, Option<string>[]>.Right([Option<string>.Some("success1"), Option<string>.None()]);
    private static readonly Either<Error, Option<string>[]> LeftErrorOptionStrings = Either<Error, Option<string>[]>.Left(Error.New("error"));
    public static readonly Either<Error, Option<string>[]>[] ErrorStringsTestData = [SomeErrorOptionStrings, LeftErrorOptionStrings];

    [TestCaseSource(nameof(ErrorStringsTestData))]
    public void GetMultiple_ShouldCallServiceWithMappedKey(Either<Error, Option<string>[]> result)
    {
        _mockService.Get<string>("mapped_key", "field1", "field2").Returns(result);

        var actual = _sut.Get<string>("key", "field1", "field2");

        actual.Should().Be(result);
        _mockService.Received(1).Get<string>("mapped_key", "field1", "field2");
    }

    [TestCaseSource(nameof(ErrorStringsTestData))]
    public async Task GetMultipleAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<string>[]> result)
    {
        _mockService.GetAsync<string>("mapped_key", "field1", "field2").Returns(result);

        var actual = await _sut.GetAsync<string>("key", "field1", "field2");

        actual.Should().Be(result);
        await _mockService.Received(1).GetAsync<string>("mapped_key", "field1", "field2");
    }

    private static readonly Either<Error, Option<object>[]> SomeErrorOptionObjects = Either<Error, Option<object>[]>.Right([Option<object>.Some(new TestData()), Option<object>.None()]);
    private static readonly Either<Error, Option<object>[]> LeftErrorOptionObjects = Either<Error, Option<object>[]>.Left(Error.New("error"));
    public static readonly Either<Error, Option<object>[]>[] ErrorObjectsTestData = [SomeErrorOptionObjects, LeftErrorOptionObjects];

    [TestCaseSource(nameof(ErrorObjectsTestData))]
    public void GetWithType_ShouldCallServiceWithMappedKey(Either<Error, Option<object>[]> result)
    {
        _mockService.Get("mapped_key", (typeof(TestData), "field1"), (typeof(TestData), "field2")).Returns(result);

        var actual = _sut.Get("key", (typeof(TestData), "field1"), (typeof(TestData), "field2"));

        actual.Should().Be(result);
        _mockService.Received(1).Get("mapped_key", (typeof(TestData), "field1"), (typeof(TestData), "field2"));
    }

    [TestCaseSource(nameof(ErrorObjectsTestData))]
    public async Task GetWithTypeAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<object>[]> result)
    {
        _mockService.GetAsync("mapped_key", (typeof(TestData), "field1"), (typeof(TestData), "field2")).Returns(result);

        var actual = await _sut.GetAsync("key", (typeof(TestData), "field1"), (typeof(TestData), "field2"));

        actual.Should().Be(result);
        await _mockService.Received(1).GetAsync("mapped_key", (typeof(TestData), "field1"), (typeof(TestData), "field2"));
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public void Set_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.Set("mapped_key", "field", Arg.Any<TestData>()).Returns(result);

        var actual = _sut.Set("key", "field", new TestData());

        actual.Should().Be(result);
        _mockService.Received(1).Set("mapped_key", "field", Arg.Any<TestData>());
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public async Task SetAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.SetAsync("mapped_key", "field", Arg.Any<TestData>()).Returns(result);

        var actual = await _sut.SetAsync("key", "field", new TestData());

        actual.Should().Be(result);
        await _mockService.Received(1).SetAsync("mapped_key", "field", Arg.Any<TestData>());
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public void SetMultiple_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.Set("mapped_key", ("field1", new TestData()), ("field2", new TestData())).Returns(result);

        var actual = _sut.Set("key", ("field1", new TestData()), ("field2", new TestData()));

        actual.Should().Be(result);
        _mockService.Received(1).Set("mapped_key", ("field1", new TestData()), ("field2", new TestData()));
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public async Task SetMultipleAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> result)
    {
        _mockService.SetAsync("mapped_key", ("field1", new TestData()), ("field2", new TestData())).Returns(result);

        var actual = await _sut.SetAsync("key", ("field1", new TestData()), ("field2", new TestData()));

        actual.Should().Be(result);
        await _mockService.Received(1).SetAsync("mapped_key", ("field1", new TestData()), ("field2", new TestData()));
    }

    private static readonly Either<Error, Option<(string, TestData)[]>> SomeErrorTuples = Either<Error, Option<(string, TestData)[]>>.Right(new[] {("first", new TestData() { Id = 1 }), ("second", new TestData() { Id = 2 })}.ToOption());
    private static readonly Either<Error, Option<(string, TestData)[]>> NoneErrorTuples = Either<Error, Option<(string, TestData)[]>>.Right(Option<(string, TestData)[]>.None());
    public static readonly Either<Error, Option<(string, TestData)[]>>[] ErrorTuplesTestData = [SomeErrorTuples, NoneErrorTuples];

    [TestCaseSource(nameof(ErrorTuplesTestData))]
    public void GetAll_ShouldCallServiceWithMappedKey(Either<Error, Option<(string, TestData)[]>> result)
    {
        _mockService.GetAll<TestData>("mapped_key").Returns(result);

        var actual = _sut.GetAll<TestData>("key");

        actual.Should().Be(result);
        _mockService.Received(1).GetAll<TestData>("mapped_key");
    }

    [TestCaseSource(nameof(ErrorTuplesTestData))]
    public async Task GetAllAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<(string, TestData)[]>> result)
    {
        _mockService.GetAllAsync<TestData>("mapped_key").Returns(result);

        var actual = await _sut.GetAllAsync<TestData>("key");

        actual.Should().Be(result);
        await _mockService.Received(1).GetAllAsync<TestData>("mapped_key");
    }

    private static readonly Either<Error, Option<TestData[]>> SomeErrorOptionTestData = Either<Error, Option<TestData[]>>.Right(new[] { new TestData() { Id = 1 }, new TestData() { Id = 2 } }.ToOption());
    private static readonly Either<Error, Option<TestData[]>> NoneErrorOptionTestData = Either<Error, Option<TestData[]>>.Right(Option<TestData[]>.None());
    public static readonly Either<Error, Option<TestData[]>>[] ErrorTestDataTestData = [SomeErrorOptionTestData, NoneErrorOptionTestData];

    [TestCaseSource(nameof(ErrorTestDataTestData))]
    public void GetValues_ShouldCallServiceWithMappedKey(Either<Error, Option<TestData[]>> result)
    {
        _mockService.GetValues<TestData>("mapped_key").Returns(result);

        var actual = _sut.GetValues<TestData>("key");

        actual.Should().Be(result);
        _mockService.Received(1).GetValues<TestData>("mapped_key");
    }

    [TestCaseSource(nameof(ErrorTestDataTestData))]
    public async Task GetValuesAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<TestData[]>> result)
    {
        _mockService.GetValuesAsync<TestData>("mapped_key").Returns(result);

        var actual = await _sut.GetValuesAsync<TestData>("key");

        actual.Should().Be(result);
        await _mockService.Received(1).GetValuesAsync<TestData>("mapped_key");
    }

    private static readonly Either<Error, Option<string[]>> SomeErrorOptionStringArray = Either<Error, Option<string[]>>.Right(new[] { "first", "second" }.ToOption());
    private static readonly Either<Error, Option<string[]>> NoneErrorOptionStringArray = Either<Error, Option<string[]>>.Right(Option<string[]>.None());
    public static readonly Either<Error, Option<string[]>>[] ErrorStringArrayTestData = [SomeErrorOptionStringArray, NoneErrorOptionStringArray];

    [TestCaseSource(nameof(ErrorStringArrayTestData))]
    public void GetFieldKeys_ShouldCallServiceWithMappedKey(Either<Error, Option<string[]>> result)
    {
        _mockService.GetFieldKeys("mapped_key").Returns(result);

        var actual = _sut.GetFieldKeys("key");

        actual.Should().Be(result);
        _mockService.Received(1).GetFieldKeys("mapped_key");
    }

    [TestCaseSource(nameof(ErrorStringArrayTestData))]
    public async Task GetFieldKeysAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<string[]>> result)
    {
        _mockService.GetFieldKeysAsync("mapped_key").Returns(result);

        var actual = await _sut.GetFieldKeysAsync("key");

        actual.Should().Be(result);
        await _mockService.Received(1).GetFieldKeysAsync("mapped_key");
    }
}
