namespace Func.Redis.Tests.RedisSetService;
internal partial class RedisSetServiceTests
{
    [Test]
    public void Size_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockSourcesProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Set.RedisSetService(_mockSourcesProvider, _mockSerDes);

        _mockSourcesProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.Size("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public void Size_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");
        _sut = new Set.RedisSetService(_mockSourcesProvider, _mockSerDes);
        _mockDb
            .SetLength("key", CommandFlags.None)
            .Returns(_ => throw exception);

        var result = _sut.Size("key");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }

    [TestCase(0)]
    [TestCase(12)]
    public void Size_WhenDatabaseReturnsZero_ShouldReturnZero(long returnValue)
    {
        _sut = new Set.RedisSetService(_mockSourcesProvider, _mockSerDes);
        _mockDb
            .SetLength("key", CommandFlags.None)
            .Returns(returnValue);

        var result = _sut.Size("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(size => size.Should().Be(returnValue));
    }

    [TestCase(0)]
    [TestCase(12)]
    public async Task SizeAsync_WhenDatabaseReturnsZero_ShouldReturnZero(long returnValue)
    {
        _sut = new Set.RedisSetService(_mockSourcesProvider, _mockSerDes);
        _mockDb
            .SetLengthAsync("key", CommandFlags.None)
            .Returns(returnValue);

        var result = await _sut.SizeAsync("key");

        result.IsRight.Should().BeTrue();
        result.OnRight(size => size.Should().Be(returnValue));
    }

}
