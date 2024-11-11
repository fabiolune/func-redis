using Func.Redis.Publisher;
using Func.Redis.SerDes;

namespace Func.Redis.Tests;

public class RedisPublisherServiceTests
{
    private RedisPublisherService _sut;
    private IRedisSerDes _mockSerDes;
    private IDatabase _mockDb;

    [SetUp]
    public void Setup()
    {
        _mockDb = Substitute.For<IDatabase>();
        _mockSerDes = Substitute.For<IRedisSerDes>();
        var mockProvider = Substitute.For<ISourcesProvider>();
        mockProvider.GetDatabase().Returns(_mockDb);

        _sut = new RedisPublisherService(mockProvider, _mockSerDes);
    }

    #region PublishMessage

    [Test]
    public void PublishMessage_ShouldReturnUnit()
    {
        _mockSerDes
            .Serialize("message")
            .Returns((RedisValue)"serialized");
        _mockDb
            .Publish(RedisChannel.Literal("some channel"), (RedisValue)"serialized", CommandFlags.None)
            .Returns(1);

        var result = _sut.Publish("some channel", "message");

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void PublishMessage_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        var exception = new Exception("some message");
        _mockSerDes
            .Serialize("message")
            .Returns((RedisValue)"serialized");
        _mockDb
            .Publish(RedisChannel.Literal("some channel"), (RedisValue)"serialized", CommandFlags.None)
            .Returns(_ => throw exception);

        var result = _sut.Publish("some channel", "message");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }

    #endregion

    #region PublishMessageAsync

    [Test]
    public async Task PublishMessageAsync_ShouldReturnUnit()
    {
        _mockSerDes
            .Serialize("message")
            .Returns((RedisValue)"serialized");
        _mockDb
            .PublishAsync(RedisChannel.Literal("some channel"), (RedisValue)"serialized", CommandFlags.None)
            .Returns(1);

        var result = await _sut.PublishAsync("some channel", "message");

        result.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task PublishMessageAsync_WhenDatabaseThrowsException_ShouldReturnRedisError()
    {
        var exception = new Exception("some message");
        _mockSerDes
            .Serialize("message")
            .Returns((RedisValue)"serialized");
        _mockDb
            .PublishAsync(RedisChannel.Literal("some channel"), (RedisValue)"serialized", CommandFlags.None)
            .Returns(_ => Task.FromException<long>(exception));

        var result = await _sut.PublishAsync("some channel", "message");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }

    #endregion
}