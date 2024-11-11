using Func.Redis.HashSet;
using Func.Redis.SerDes.Json;

namespace Func.Redis.IntegrationTests.HashSet;
internal abstract class RedisHashSetServiceIntegrationTest(string redisImage) : RedisIntegrationTestBase(redisImage)
{
    private RedisHashSetService _sut;

    [OneTimeSetUp]
    public override async Task OneTimeSetUp()
    {
        await base.OneTimeSetUp();

        _sut = _provider
            .Map(sp => new RedisHashSetService(sp, new SystemJsonRedisSerDes()));
    }

    [Test]
    public async Task WhenDataAreNotPresent_GetShouldReturnNone()
    {
        var getResult = await _sut.GetAsync<TestModel>("some key");

        getResult.IsRight.Should().BeTrue();
        getResult.OnRight(o => o.Should().BeEmpty());
    }

    [Test]
    public async Task WhenDataAreSuccessfullyAdded_TheyShouldBeSuccessfullyRetrieved()
    {
        var input1 = new TestModel
        {
            Id = Guid.NewGuid()
        };

        var input2 = new TestModel
        {
            Id = Guid.NewGuid()
        };

        var insertResult1 = await _sut.SetAsync("key", "field1", input1);
        insertResult1.IsRight.Should().BeTrue();

        var insertResult2 = await _sut.SetAsync("key", "field2", input2);
        insertResult2.IsRight.Should().BeTrue();

        var getResult1 = await _sut.GetAsync<TestModel>("key", "field1");
        getResult1.IsRight.Should().BeTrue();
        getResult1.OnRight(o =>
        {
            o.IsSome.Should().BeTrue();
            o.OnSome(v => v.Should().BeEquivalentTo(input1));
        });

        var getResult2 = await _sut.GetAsync<TestModel>("key", "field2");
        getResult2.IsRight.Should().BeTrue();
        getResult2.OnRight(o =>
        {
            o.IsSome.Should().BeTrue();
            o.OnSome(v => v.Should().BeEquivalentTo(input2));
        });

        var getResult = await _sut.GetAsync<TestModel>("key", "field1", "field2");
        getResult.IsRight.Should().BeTrue();
        getResult.OnRight(o => o.Should().BeEquivalentTo([
            Option<TestModel>.Some(input1),
            Option<TestModel>.Some(input2)
        ]));

        var getResultWithMisingField = await _sut.GetAsync<TestModel>("key", "field1", "field3");
        getResultWithMisingField.IsRight.Should().BeTrue();
        getResultWithMisingField.OnRight(o => o.Should().BeEquivalentTo([
            Option<TestModel>.Some(input1),
            Option<TestModel>.None()
        ]));
    }
}
