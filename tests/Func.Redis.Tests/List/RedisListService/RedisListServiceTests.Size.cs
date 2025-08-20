namespace Func.Redis.Tests.RedisListService;
internal partial class RedisListServiceTests
{
    [Test]
    public void Size_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");
        _mockDb
            .ListLength("key", CommandFlags.None)
            .Returns(_ => throw exception);

        var result = _sut.Size("key");

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
    }

    [Test]
    public async Task SizeAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");
        _mockDb
            .ListLengthAsync("key", CommandFlags.None)
            .Returns<long>(_ => throw exception);

        var result = await _sut.SizeAsync("key");

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
    }

    [TestCase(0)]
    [TestCase(12)]
    public void Size_WhenDatabaseReturnsValue_ShouldReturnValue(long returnValue)
    {
        _mockDb
            .ListLength("key", CommandFlags.None)
            .Returns(returnValue);

        var result = _sut.Size("key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(size => size.ShouldBe(returnValue));
    }

    [TestCase(0)]
    [TestCase(12)]
    public async Task SizeAsync_WhenDatabaseReturnsValue_ShouldReturnValue(long returnValue)
    {
        _mockDb
            .ListLengthAsync("key", CommandFlags.None)
            .Returns(returnValue);

        var result = await _sut.SizeAsync("key");

        result.IsRight.ShouldBeTrue();
        result.OnRight(size => size.ShouldBe(returnValue));
    }
}