namespace Func.Redis.Tests.RedisKeyService;

public partial class RedisKeyServiceTests
{
    [Test]
    public async Task DeleteAsync_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockSourcesProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.RedisKeyService(_mockSourcesProvider, _mockSerDes);

        _mockSourcesProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = await _sut.DeleteAsync("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public async Task MultipleDeleteAsync_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockSourcesProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.RedisKeyService(_mockSourcesProvider, _mockSerDes);

        _mockSourcesProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = await _sut.DeleteAsync("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public async Task DeleteAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");
        
        _mockDb
            .KeyDeleteAsync("key", Arg.Any<CommandFlags>())
            .Returns<bool>(_ => throw exception);

        var result = await _sut.DeleteAsync("key");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .KeyDeleteAsync("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public async Task MultipleDeleteAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        
        _mockDb
            .KeyDeleteAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns<long>(_ => throw exception);

        var result = await _sut.DeleteAsync("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .KeyDeleteAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task DeleteAsync_WhenDatabaseReturnsValidBool_ShouldReturnRightWithSome(bool returnValue)
    {
        _mockDb
            .KeyDeleteAsync("key", Arg.Any<CommandFlags>())
            .Returns(returnValue);

        var result = await _sut.DeleteAsync("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.Should().Be(Unit.Default));

        await _mockDb
            .Received(1)
            .KeyDeleteAsync("key", Arg.Any<CommandFlags>());
    }

    [TestCase(0)]
    [TestCase(11)]
    public async Task MultipleDeleteAsync_WhenDatabaseReturnsValidBool_ShouldReturnRightWithSome(long returnValue)
    {
        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        _mockDb
            .KeyDeleteAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns(returnValue);

        var result = await _sut.DeleteAsync("key1", "key2");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.Should().Be(Unit.Default));

        await _mockDb
            .Received(1)
            .KeyDeleteAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }
}