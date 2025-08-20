namespace Func.Redis.Tests.RedisKeyService;

public partial class RedisKeyServiceTests
{
    [Test]
    public async Task DeleteAsync_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockSourcesProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.Key.RedisKeyService(_mockSourcesProvider, _mockSerDes);

        _mockSourcesProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = await _sut.DeleteAsync("key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(err => err.ShouldBe(Error.New(new NullReferenceException())));
    }

    [Test]
    public async Task MultipleDeleteAsync_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockSourcesProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.Key.RedisKeyService(_mockSourcesProvider, _mockSerDes);

        _mockSourcesProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = await _sut.DeleteAsync("key1", "key2");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(err => err.ShouldBe(Error.New(new NullReferenceException())));
    }

    [Test]
    public async Task DeleteAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .KeyDeleteAsync("key", Arg.Any<CommandFlags>())
            .Returns<bool>(_ => throw exception);

        var result = await _sut.DeleteAsync("key");

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
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

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
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

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e => e.ShouldBe(Unit.Default));

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

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e => e.ShouldBe(Unit.Default));

        await _mockDb
            .Received(1)
            .KeyDeleteAsync(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }
}