namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [TestCase("")]
    [TestCase(null)]
    [TestCase("something")]
    public async Task GetKeysAsync_WhenDatabaseIsNull_ShouldReturnError(string prefix)
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = await _sut.GetFieldKeysAsync("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("  :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public async Task GetKeysAsync_WhenDatabaseThrowsException_ShouldReturnError(string prefix, string key)
    {
        var exception = new Exception("some message");
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashKeysAsync(key, Arg.Any<CommandFlags>())
            .Returns<RedisValue[]>(_ => throw exception);

        var result = await _sut.GetFieldKeysAsync("key");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        await _mockDb
            .Received(1)
            .HashKeysAsync(key, Arg.Any<CommandFlags>());
    }

    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("  :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    [TestCase("", "key")]
    [TestCase(":", "key")]
    [TestCase("  :", "key")]
    [TestCase("::", "key")]
    [TestCase("prefix", "prefix:key")]
    [TestCase("prefix:", "prefix:key")]
    [TestCase("prefix::", "prefix:key")]
    public async Task GetKeysAsync_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone(string prefix, string key)
    {
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashKeysAsync(key, Arg.Any<CommandFlags>())
            .Returns([]);

        var result = await _sut.GetFieldKeysAsync("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        await _mockDb
            .Received(1)
            .HashKeysAsync(key, Arg.Any<CommandFlags>());
    }

    [TestCase("some-id", "some-id2", "", "key")]
    public async Task GetKeysAsync_WhenDatabaseReturnsMoreString_ShouldReturnRightWithSome(string serializedData, string serializedData2, string prefix, string key)
    {
        var expected = new[] { "some-id", "some-id2" };
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes, new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        });
        _mockDb
            .HashKeysAsync(key, Arg.Any<CommandFlags>())
            .Returns([serializedData, serializedData2]);

        var result = await _sut.GetFieldKeysAsync("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.OnSome(d => d.Should().BeEquivalentTo(expected)));
    }
}