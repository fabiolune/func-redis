namespace Func.Redis.Tests.RedisKeyService;

public partial class RedisKeyServiceTests
{
    [Test]

    public void Delete_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockSourcesProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.Key.RedisKeyService(_mockSourcesProvider, _mockSerDes);

        _mockSourcesProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.Delete("key");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(err => err.ShouldBe(Error.New(new NullReferenceException())));
    }

    [Test]
    public void MultipleDelete_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockSourcesProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.Key.RedisKeyService(_mockSourcesProvider, _mockSerDes);

        _mockSourcesProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.Delete("key1", "key2");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(err => err.ShouldBe(Error.New(new NullReferenceException())));
    }

    [Test]
    public void Delete_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");
        _sut = new Redis.Key.RedisKeyService(_mockSourcesProvider, _mockSerDes);
        _mockDb
            .KeyDelete("key", Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Delete("key");

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
        _mockDb
            .Received(1)
            .KeyDelete("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public void MultipleDelete_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");
        var keys = new[] { (RedisKey)"key1", (RedisKey)"key2" };
        _sut = new Redis.Key.RedisKeyService(_mockSourcesProvider, _mockSerDes);
        _mockDb
            .KeyDelete(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Delete("key1", "key2");

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
        _mockDb
            .Received(1)
            .KeyDelete(Arg.Is<RedisKey[]>(k => k.SequenceEqual(keys)), Arg.Any<CommandFlags>());
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Delete_WhenDatabaseReturnsValidBool_ShouldReturnRightWithSome(bool returnValue)
    {
        _sut = new Redis.Key.RedisKeyService(_mockSourcesProvider, _mockSerDes);
        _mockDb
            .KeyDelete("key", Arg.Any<CommandFlags>())
            .Returns(returnValue);

        var result = _sut.Delete("key");

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e => e.ShouldBe(Unit.Default));
    }

    [TestCase(0)]
    [TestCase(11)]
    public void MultipleDelete_WhenDatabaseReturnsValidBool_ShouldReturnRightWithSome(long returnValue)
    {
        _sut = new Redis.Key.RedisKeyService(_mockSourcesProvider, _mockSerDes);
        _mockDb
            .KeyDelete(Arg.Is<RedisKey[]>(a =>
                a.SequenceEqual(new[] { (RedisKey)"key1", (RedisKey)"key2" })), Arg.Any<CommandFlags>())
            .Returns(returnValue);

        var result = _sut.Delete("key1", "key2");

        result.IsRight.ShouldBeTrue();
    }
}