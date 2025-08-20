using Func.Redis.Publisher;
using Func.Redis.Subscriber;
using StackExchange.Redis;

namespace Func.Redis.IntegrationTests.PubSub;
internal abstract class PubSubIntegrationTest(string redisImage) : RedisIntegrationTestBase(redisImage)
{
    private class TestSubscriber(Action<Option<TestModel>> action, string channel) : IRedisSubscriber
    {
        private readonly SystemJsonRedisSerDes _serDes = new();
        private readonly Action<Option<TestModel>> _action = action;
        private readonly string _channel = channel;

        public (string, Action<RedisChannel, RedisValue>) GetSubscriptionHandler() =>
            (_channel, (c, v) => _action(_serDes.Deserialize<TestModel>(v)));
    }

    private RedisPublisherService _sut;

    [OneTimeSetUp]
    public override async Task OneTimeSetUp()
    {
        await base.OneTimeSetUp();

        _sut = _provider.Map(sp => new RedisPublisherService(sp, new SystemJsonRedisSerDes()));
    }

    [Test]
    public async Task WhenDataArePublished_TheyShouldBeSuccessfullyReceivedByProperSubscribers()
    {
        const string channel = "some channel";
        var called1 = false;
        TestModel receivedData1 = null;
        var subscriber1 = new TestSubscriber(o =>
        {
            called1 = true;
            o.IsSome.ShouldBeTrue();
            o.OnSome(d => receivedData1 = d);
        }, channel);

        var called2 = false;
        TestModel receivedData2 = null;
        var subscriber2 = new TestSubscriber(o =>
        {
            called2 = true;
            o.IsSome.ShouldBeTrue();
            o.OnSome(d => receivedData2 = d);
        }, "different channel");

        await subscriber1
            .GetSubscriptionHandler()
            .Map(t =>
                _connectionMultiplexerProvider
                .GetMultiplexer()
                .GetSubscriber()
                .SubscribeAsync(RedisChannel.Literal(t.Item1), t.Item2));

        await subscriber2
            .GetSubscriptionHandler()
            .Map(t =>
                _connectionMultiplexerProvider
                .GetMultiplexer()
                .GetSubscriber()
                .SubscribeAsync(RedisChannel.Literal(t.Item1), t.Item2));

        var data = new TestModel
        {
            Id = Guid.NewGuid()
        };
        var publishResult = await _sut.PublishAsync(channel, data);

        publishResult.IsRight.ShouldBeTrue();

        await Task.Delay(500);
        called1.ShouldBeTrue();
        receivedData1.ShouldBeEquivalentTo(data);

        called2.ShouldBeFalse();
        receivedData2.ShouldBeNull();
    }
}
