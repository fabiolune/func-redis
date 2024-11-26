using Func.Redis.List;
using static Func.Redis.Tests.TestDataElements;

namespace Func.Redis.Tests.List;
internal class KeyTransformerRedisListServiceTests
{
    private KeyTransformerRedisListService _sut;
    private IRedisListService _mockService;

    [SetUp]
    public void Setup()
    {
        _mockService = Substitute.For<IRedisListService>();
        _sut = new KeyTransformerRedisListService(_mockService, k => $"mapped_{k}");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void Append_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var value = new TestData(1);
        _mockService.Append("mapped_key", value).Returns(internalResult);

        var result = _sut.Append(key, value);

        result.Should().Be(internalResult);
        _mockService.Received(1).Append("mapped_key", value);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void AppendMultiple_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var values = new[] { new TestData(1), new TestData(2) };
        _mockService.Append("mapped_key", values).Returns(internalResult);

        var result = _sut.Append(key, values);

        result.Should().Be(internalResult);
        _mockService.Received(1).Append("mapped_key", values);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task AppendAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var value = new TestData(1);
        _mockService.AppendAsync("mapped_key", value).Returns(internalResult);

        var result = await _sut.AppendAsync(key, value);

        result.Should().Be(internalResult);
        await _mockService.Received(1).AppendAsync("mapped_key", value);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task AppendMultipleAsync_ShouldCalServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var values = new[] { new TestData(1), new TestData(2) };
        _mockService.AppendAsync("mapped_key", values).Returns(internalResult);

        var result = await _sut.AppendAsync(key, values);

        result.Should().Be(internalResult);
        await _mockService.Received(1).AppendAsync("mapped_key", values);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void Prepend_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var value = new TestData(1);
        _mockService.Prepend("mapped_key", value).Returns(internalResult);

        var result = _sut.Prepend(key, value);

        result.Should().Be(internalResult);
        _mockService.Received(1).Prepend("mapped_key", value);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public void PrependMultiple_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var values = new[] { new TestData(1), new TestData(2) };
        _mockService.Prepend("mapped_key", values).Returns(internalResult);

        var result = _sut.Prepend(key, values);

        result.Should().Be(internalResult);
        _mockService.Received(1).Prepend("mapped_key", values);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task PrependAsync_ShouldCallServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var value = new TestData(1);
        _mockService.PrependAsync("mapped_key", value).Returns(internalResult);

        var result = await _sut.PrependAsync(key, value);

        result.Should().Be(internalResult);
        await _mockService.Received(1).PrependAsync("mapped_key", value);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorUnitTestData))]
    public async Task PrependMultipleAsync_ShouldCalServiceWithMappedKey(Either<Error, Unit> internalResult)
    {
        var key = "key";
        var values = new[] { new TestData(1), new TestData(2) };
        _mockService.PrependAsync("mapped_key", values).Returns(internalResult);

        var result = await _sut.PrependAsync(key, values);

        result.Should().Be(internalResult);
        await _mockService.Received(1).PrependAsync("mapped_key", values);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringTestData))]
    public void Get_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> internalResult)
    {
        var key = "key";
        _mockService.Get<string>("mapped_key", 12).Returns(internalResult);

        var result = _sut.Get<string>(key, 12);

        result.Should().Be(internalResult);
        _mockService.Received(1).Get<string>("mapped_key", 12);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorObjectsTestData))]
    public void MultipleGet_ShouldCallServiceWithMappedKey(Either<Error, Option<object>[]> internalResult)
    {
        var key = "key";
        _mockService.Get<object>("mapped_key", 12, 20).Returns(internalResult);

        var result = _sut.Get<object>(key, 12, 20);

        result.Should().Be(internalResult);
        _mockService.Received(1).Get<object>("mapped_key", 12, 20);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringTestData))]
    public async Task GetAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> internalResult)
    {
        var key = "key";
        _mockService.GetAsync<string>("mapped_key", 12).Returns(internalResult);

        var result = await _sut.GetAsync<string>(key, 12);

        result.Should().Be(internalResult);
        await _mockService.Received(1).GetAsync<string>("mapped_key", 12);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorObjectsTestData))]
    public async Task MultipleGetAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<object>[]> internalResult)
    {
        var key = "key";
        _mockService.GetAsync<object>("mapped_key", 12, 20).Returns(internalResult);

        var result = await _sut.GetAsync<object>(key, 12, 20);

        result.Should().Be(internalResult);
        await _mockService.Received(1).GetAsync<object>("mapped_key", 12, 20);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringTestData))]
    public void Pop_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> internalResult)
    {
        var key = "key";
        _mockService.Pop<string>("mapped_key").Returns(internalResult);

        var result = _sut.Pop<string>(key);

        result.Should().Be(internalResult);
        _mockService.Received(1).Pop<string>("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorObjectsTestData))]
    public void CountPop_ShouldCallServiceWithMappedKey(Either<Error, Option<object>[]> internalResult)
    {
        var key = "key";
        _mockService.Pop<object>("mapped_key", 12).Returns(internalResult);

        var result = _sut.Pop<object>(key, 12);

        result.Should().Be(internalResult);
        _mockService.Received(1).Pop<object>("mapped_key", 12);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringTestData))]
    public async Task PopAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> internalResult)
    {
        var key = "key";
        _mockService.PopAsync<string>("mapped_key").Returns(internalResult);

        var result = await _sut.PopAsync<string>(key);

        result.Should().Be(internalResult);
        await _mockService.Received(1).PopAsync<string>("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorObjectsTestData))]
    public async Task CountPopAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<object>[]> internalResult)
    {
        var key = "key";
        _mockService.PopAsync<object>("mapped_key", 12).Returns(internalResult);

        var result = await _sut.PopAsync<object>(key, 12);

        result.Should().Be(internalResult);
        await _mockService.Received(1).PopAsync<object>("mapped_key", 12);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringTestData))]
    public void Shift_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> internalResult)
    {
        var key = "key";
        _mockService.Shift<string>("mapped_key").Returns(internalResult);

        var result = _sut.Shift<string>(key);

        result.Should().Be(internalResult);
        _mockService.Received(1).Shift<string>("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorObjectsTestData))]
    public void CountShift_ShouldCallServiceWithMappedKey(Either<Error, Option<object>[]> internalResult)
    {
        var key = "key";
        _mockService.Shift<object>("mapped_key", 12).Returns(internalResult);

        var result = _sut.Shift<object>(key, 12);

        result.Should().Be(internalResult);
        _mockService.Received(1).Shift<object>("mapped_key", 12);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorStringTestData))]
    public async Task ShiftAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<string>> internalResult)
    {
        var key = "key";
        _mockService.ShiftAsync<string>("mapped_key").Returns(internalResult);

        var result = await _sut.ShiftAsync<string>(key);

        result.Should().Be(internalResult);
        await _mockService.Received(1).ShiftAsync<string>("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorObjectsTestData))]
    public async Task CountShiftAsync_ShouldCallServiceWithMappedKey(Either<Error, Option<object>[]> internalResult)
    {
        var key = "key";
        _mockService.ShiftAsync<object>("mapped_key", 12).Returns(internalResult);

        var result = await _sut.ShiftAsync<object>(key, 12);

        result.Should().Be(internalResult);
        await _mockService.Received(1).ShiftAsync<object>("mapped_key", 12);
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorLongTestData))]
    public void Size_ShouldCallServiceWithMappedKey(Either<Error, long> internalResult)
    {
        var key = "key";
        _mockService.Size("mapped_key").Returns(internalResult);

        var result = _sut.Size(key);

        result.Should().Be(internalResult);
        _mockService.Received(1).Size("mapped_key");
    }

    [TestCaseSource(typeof(TestDataElements), nameof(ErrorLongTestData))]
    public async Task SizeAsync_ShouldCallServiceWithMappedKey(Either<Error, long> internalResult)
    {
        var key = "key";
        _mockService.SizeAsync("mapped_key").Returns(internalResult);

        var result = await _sut.SizeAsync(key);

        result.Should().Be(internalResult);
        await _mockService.Received(1).SizeAsync("mapped_key");
    }
}