using Func.Redis.Key;

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

        getResult.IsRight.ShouldBeTrue();
        getResult.OnRight(o => o.IsNone.ShouldBeTrue());
    }

    [Test]
    public async Task WhenDataAreSuccessfullyAdded_TheyShouldBeSuccessfullyRetrieved()
    {
        var key = nameof(WhenDataAreSuccessfullyAdded_TheyShouldBeSuccessfullyRetrieved);

        var input = new TestModel
        {
            Id = Guid.NewGuid()
        };

        var insertResult = await _sut.SetAsync(key, input);

        insertResult.IsRight.ShouldBeTrue();

        var getResult = await _sut.GetAsync<TestModel>(key);

        getResult.IsRight.ShouldBeTrue();
        getResult.OnRight(o =>
        {
            o.IsSome.ShouldBeTrue();
            o.OnSome(v => v.ShouldBeEquivalentTo(input));
        });

        var deleteResult = await _sut.DeleteAsync(key);

        deleteResult.IsRight.ShouldBeTrue();

        var getResultAfterDelete = await _sut.GetAsync<TestModel>(key);

        getResultAfterDelete.IsRight.ShouldBeTrue();
        getResultAfterDelete.OnRight(o => o.IsNone.ShouldBeTrue());
    }
}
