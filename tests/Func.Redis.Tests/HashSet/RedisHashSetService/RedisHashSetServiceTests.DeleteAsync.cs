namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [Test]
    public async Task DeleteAsync_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.HashSet.RedisHashSetService(_mockProvider, _mockSerDes);

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = await _sut.DeleteAsync("key", "field");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public async Task DeleteAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");
        _mockDb
            .HashDeleteAsync("key", "field", Arg.Any<CommandFlags>())
            .Returns<bool>(_ => throw exception);

        var result = await _sut.DeleteAsync("key", "field");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Message.Should().Be("some message"));
        await _mockDb
            .Received(1)
            .HashDeleteAsync("key", "field", Arg.Any<CommandFlags>());
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task DeleteAsync_WhenDatabaseReturnsValidBool_ShouldReturnRightWithSome(bool returnValue)
    {
        _mockDb
            .HashDeleteAsync("key", "field", Arg.Any<CommandFlags>())
            .Returns(returnValue);

        var result = await _sut.DeleteAsync("key", "field");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.Should().Be(Unit.Default));
    }

    [Test]
    public async Task MultipleDeleteAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var _params = new[] { "field1", "field2" };
        var exception = new Exception("some message");

        _mockDb
            .HashDeleteAsync("key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns<long>(_ => throw exception);

        var result = await _sut.DeleteAsync("key", _params);

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .HashDeleteAsync((RedisKey)"key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>());
    }

    [TestCase(0)]
    [TestCase(11)]
    public async Task MultipleDeleteAsync_WhenDatabaseReturnsValidBool_ShouldReturnRightWithSome(long returnValue)
    {
        var fields = new[] { (RedisValue)"field1", (RedisValue)"field2" };
        var requestParams = new[] { "field1", "field2" };

        _mockDb
            .HashDeleteAsync((RedisKey)"key", Arg.Is<RedisValue[]>(v => v.SequenceEqual(fields)), Arg.Any<CommandFlags>())
            .Returns(returnValue.AsTask());

        var result = await _sut.DeleteAsync("key", requestParams);

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.Should().Be(Unit.Default));
    }
}