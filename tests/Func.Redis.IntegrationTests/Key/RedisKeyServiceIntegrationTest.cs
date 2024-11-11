using Func.Redis.Key;
using Func.Redis.SerDes.Json;

namespace Func.Redis.IntegrationTests.Key;

internal abstract class RedisKeyServiceIntegrationTest(string redisImage) : RedisIntegrationTestBase(redisImage)
{
    private RedisKeyService _sut;

    [OneTimeSetUp]
    public override async Task OneTimeSetUp()
    {
        await base.OneTimeSetUp();

        _sut = _provider.Map(sp => new RedisKeyService(sp, new SystemJsonRedisSerDes()));
    }

    [Test]
    public async Task WhenDataAreNotPresent_GetShouldReturnNone()
    {
        var getResult = await _sut.GetAsync<TestModel>("some key");

        getResult.IsRight.Should().BeTrue();
        getResult.OnRight(o => o.IsNone.Should().BeTrue());
    }

    [Test]
    public async Task WhenDataAreSuccessfullyAdded_TheyShouldBeSuccessfullyRetrieved()
    {
        var input = new TestModel
        {
            Id = Guid.NewGuid()
        };

        var insertResult = await _sut.SetAsync("key", input);

        insertResult.IsRight.Should().BeTrue();

        var getResult = await _sut.GetAsync<TestModel>("key");

        getResult.IsRight.Should().BeTrue();
        getResult.OnRight(o =>
        {
            o.IsSome.Should().BeTrue();
            o.OnSome(v => v.Should().BeEquivalentTo(input));
        });

        var deleteResult = await _sut.DeleteAsync("key");

        deleteResult.IsRight.Should().BeTrue();

        var getResultAfterDelete = await _sut.GetAsync<TestModel>("key");

        getResultAfterDelete.IsRight.Should().BeTrue();
        getResultAfterDelete.OnRight(o => o.IsNone.Should().BeTrue());
    }
}
