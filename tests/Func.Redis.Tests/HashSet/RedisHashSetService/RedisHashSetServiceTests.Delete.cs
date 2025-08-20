namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [Test]
    public void Delete_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.HashSet.RedisHashSetService(_mockProvider, _mockSerDes);

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.Delete("key", "field");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(err => err.ShouldBe(Error.New(new NullReferenceException())));
    }

    [Test]
    public void Delete_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .HashDelete("key", "field", Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Delete("key", "field");

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(err => err.Message.ShouldBe("some message"));
        _mockDb
            .Received(1)
            .HashDelete("key", "field", Arg.Any<CommandFlags>());
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Delete_WhenDatabaseReturnsValidBool_ShouldReturnRightWithSome(bool returnValue)
    {
        _mockDb
            .HashDelete("key", "field", Arg.Any<CommandFlags>())
            .Returns(returnValue);

        var result = _sut.Delete("key", "field");

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e => e.ShouldBe(Unit.Default));
    }

    [Test]
    public void MultipleDelete_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var parameters = new[] { "field1", "field2" };
        var exception = new Exception("some message");

        _mockDb
            .HashDelete("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.Delete("key", parameters);

        result.IsLeft.ShouldBeTrue();
        result
            .OnLeft(e => e.ShouldBeEquivalentTo(Error.New(exception)));
        _mockDb
            .Received(1)
            .HashDelete((RedisKey)"key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase(0)]
    [TestCase(11)]
    public void MultipleDelete_WhenDatabaseReturnsValidBool_ShouldReturnRightWithSome(long returnValue)
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var _params = new[] { "field1", "field2" };

        _mockDb
            .HashDelete((RedisKey)"key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(returnValue);

        var result = _sut.Delete("key", _params);

        result.IsRight.ShouldBeTrue();
        result
            .OnRight(e => e.ShouldBe(Unit.Default));
    }
}