namespace Func.Redis.Tests;

[TestFixture]
public class KeyTransformerRedisKeyServiceTests
{
    private IRedisKeyService _mockService;
    private KeyTransformerRedisKeyService _sut;

    private static readonly Either<Error, Unit> RightErrorUnit = Either<Error, Unit>.Right(Unit.Default);
    private static readonly Either<Error, Unit> LeftErrorUnit = Either<Error, Unit>.Left(Error.New("error"));
    public static readonly Either<Error, Unit>[] ErrorUnitTestData = [RightErrorUnit, LeftErrorUnit];

    [SetUp]
    public void SetUp()
    {
        _mockService = Substitute.For<IRedisKeyService>();
        _sut = new KeyTransformerRedisKeyService(_mockService, k => $"mapped_{k}", k => k.Replace("mapped_", ""));
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public void Delete_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.Delete("mapped_key").Returns(internalResult);

        var result = _sut.Delete(key);

        result.Should().Be(internalResult);
        _mockService.Received(1).Delete("mapped_key");
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public void DeleteMultiple_ShouldCallServiceWithMappedKeys(Either<Error, Unit> internalResult)
    {
        var mappedKeys = new[] { "mapped_key1", "mapped_key2" };
        _mockService.Delete("mapped_key1", "mapped_key2").Returns(internalResult);

        var result = _sut.Delete("key1", "key2");

        result.Should().Be(internalResult);
        _mockService.Received(1).Delete(Arg.Is<string[]>(k => k.SequenceEqual(mappedKeys)));
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public async Task DeleteAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        _mockService.DeleteAsync("mapped_key").Returns(internalResult);

        var result = await _sut.DeleteAsync(key);

        result.Should().Be(internalResult);
        await _mockService.Received(1).DeleteAsync("mapped_key");
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public async Task DeleteMultipleAsync_ShouldCallServiceWithMappedKeys(Either<Error, Unit> internalResult)
    {
        var keys = new[] { "key1", "key2" };
        var mappedKeys = new[] { "mapped_key1", "mapped_key2" };
        _mockService.DeleteAsync("mapped_key1", "mapped_key2").Returns(internalResult);

        var result = await _sut.DeleteAsync(keys);

        result.Should().Be(internalResult);
        await _mockService.Received(1).DeleteAsync(Arg.Is<string[]>(k => k.SequenceEqual(mappedKeys)));
    }

    private static readonly Either<Error, Option<string>> SomeErrorOptionString = Either<Error, Option<string>>.Right(Option<string>.Some("success"));
    private static readonly Either<Error, Option<string>> NoneErrorOptionString = Either<Error, Option<string>>.Right(Option<string>.None());
    private static readonly Either<Error, Option<string>> LeftErrorOptionString = Either<Error, Option<string>>.Left(Error.New("error"));
    public static readonly Either<Error, Option<string>>[] ErrorStringTestData = [SomeErrorOptionString, NoneErrorOptionString, LeftErrorOptionString];

    [TestCaseSource(nameof(ErrorStringTestData))]
    public void Get_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> internalResult)
    {
        var key = "key";
        _mockService.Get<string>("mapped_key").Returns(internalResult);

        var result = _sut.Get<string>(key);

        result.Should().Be(internalResult);
        _mockService.Received(1).Get<string>("mapped_key");
    }

    [TestCaseSource(nameof(ErrorStringTestData))]
    public async Task GetAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> internalResult)
    {
        var key = "key";
        _mockService.GetAsync<string>("mapped_key").Returns(internalResult);

        var result = await _sut.GetAsync<string>(key);

        result.Should().Be(internalResult);
        await _mockService.Received(1).GetAsync<string>("mapped_key");
    }

    private static readonly Either<Error, Option<string>[]> SomeErrorOptionStrings = Either<Error, Option<string>[]>.Right([Option<string>.Some("success1"), Option<string>.None()]);
    private static readonly Either<Error, Option<string>[]> LeftErrorOptionStrings = Either<Error, Option<string>[]>.Left(Error.New("error"));
    public static readonly Either<Error, Option<string>[]>[] ErrorStringsTestData = [SomeErrorOptionStrings, LeftErrorOptionStrings];

    [TestCaseSource(nameof(ErrorStringsTestData))]
    public void GetMultiple_ShouldCallServiceWithMappedKeys(Either<Error, Option<string>[]> internalResult)
    {
        var keys = new[] { "key1", "key2" };
        var mappedKeys = new[] { "mapped_key1", "mapped_key2" };
        _mockService.Get<string>(mappedKeys).Returns(internalResult);

        var result = _sut.Get<string>(keys);

        result.Should().Be(internalResult);
        _mockService.Received(1).Get<string>(mappedKeys);
    }

    [TestCaseSource(nameof(ErrorStringsTestData))]
    public async Task GetMultipleAsync_ShouldCallServiceWithMappedKeys(Either<Error, Option<string>[]> internalResult)
    {
        var keys = new[] { "key1", "key2" };
        var mappedKeys = new[] { "mapped_key1", "mapped_key2" };
        _mockService.GetAsync<string>(mappedKeys).Returns(internalResult);

        var result = await _sut.GetAsync<string>(keys);

        result.Should().Be(internalResult);
        await _mockService.Received(1).GetAsync<string>(mappedKeys);
    }

    [Test]
    public void GetKeys_ShouldCallServiceWithMappedPatternAndUseInverseKeyMapper()
    {
        var pattern = "pattern";
        var mappedKeys = new[] { "mapped_key1", "mapped_key2" };
        var keys = new[] { "key1", "key2" };
        _mockService.GetKeys("mapped_pattern").Returns(mappedKeys);

        var result = _sut.GetKeys(pattern);

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.Should().BeEquivalentTo(keys));
    }

    [Test]
    public async Task GetKeysAsync_ShouldCallServiceWithMappedPatternAndUseInverseKeyMapper()
    {
        var pattern = "pattern";
        var mappedKeys = new[] { "mapped_key1", "mapped_key2" };
        var keys = new[] { "key1", "key2" };
        _mockService.GetKeysAsync("mapped_pattern").Returns(mappedKeys);

        var result = await _sut.GetKeysAsync(pattern);

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.Should().BeEquivalentTo(keys));
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public void RenameKey_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var newKey = "newKey";
        _mockService.RenameKey("mapped_key", "mapped_newKey").Returns(internalResult);

        var result = _sut.RenameKey(key, newKey);

        result.Should().Be(internalResult);
        _mockService.Received(1).RenameKey("mapped_key", "mapped_newKey");
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public async Task RenameKeyAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var newKey = "newKey";
        _mockService.RenameKeyAsync("mapped_key", "mapped_newKey").Returns(internalResult);

        var result = await _sut.RenameKeyAsync(key, newKey);

        result.Should().Be(internalResult);
        await _mockService.Received(1).RenameKeyAsync("mapped_key", "mapped_newKey");
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public void Set_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var value = "value";
        _mockService.Set("mapped_key", value).Returns(internalResult);

        var result = _sut.Set(key, value);

        result.Should().Be(internalResult);
        _mockService.Received(1).Set("mapped_key", value);
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public void SetMultiple_ShouldCallServiceWithMappedKeys(Either<Error, Unit> internalResult)
    {
        var pairs = new[] { ("key1", "value1"), ("key2", "value2") };
        _mockService.Set(Arg.Is<(string, string)[]>(p =>
                p.Length == 2
                && p[0].Item1 == "mapped_key1"
                && p[0].Item2 == "value1"
                && p[1].Item1 == "mapped_key2"
                && p[1].Item2 == "value2"))
            .Returns(internalResult);

        var result = _sut.Set(pairs);

        result.Should().Be(internalResult);
        _mockService.Received(1).Set(Arg.Is<(string, string)[]>(p =>
                p.Length == 2
                && p[0].Item1 == "mapped_key1"
                && p[0].Item2 == "value1"
                && p[1].Item1 == "mapped_key2"
                && p[1].Item2 == "value2"));
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public async Task SetAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var value = "value";
        _mockService.SetAsync("mapped_key", value).Returns(internalResult);

        var result = await _sut.SetAsync(key, value);

        result.Should().Be(internalResult);
        await _mockService.Received(1).SetAsync("mapped_key", value);
    }

    [TestCaseSource(nameof(ErrorUnitTestData))]
    public async Task SetMultipleAsync_ShouldCallServiceWithMappedKeys(Either<Error, Unit> internalResult)
    {
        var pairs = new[] { ("key1", "value1"), ("key2", "value2") };
        _mockService.SetAsync(Arg.Is<(string, string)[]>(p =>
                p.Length == 2
                && p[0].Item1 == "mapped_key1"
                && p[0].Item2 == "value1"
                && p[1].Item1 == "mapped_key2"
                && p[1].Item2 == "value2"))
            .Returns(internalResult);

        var result = await _sut.SetAsync(pairs);

        result.Should().Be(internalResult);
        await _mockService.Received(1).SetAsync(Arg.Is<(string, string)[]>(p =>
                p.Length == 2
                && p[0].Item1 == "mapped_key1"
                && p[0].Item2 == "value1"
                && p[1].Item1 == "mapped_key2"
                && p[1].Item2 == "value2"));
    }
}
