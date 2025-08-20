using Func.Redis.HashSet;

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

        getResult.IsRight.ShouldBeTrue();
        getResult.OnRight(o => o.ShouldBeEmpty());
    }

    [Test]
    public async Task WhenDataAreSuccessfullyAdded_TheyShouldBeSuccessfullyRetrieved()
    {
        var key = nameof(WhenDataAreSuccessfullyAdded_TheyShouldBeSuccessfullyRetrieved);

        var input1 = new TestModel
        {
            Id = Guid.NewGuid()
        };

        var input2 = new TestModel
        {
            Id = Guid.NewGuid()
        };

        var insertResult1 = await _sut.SetAsync(key, "field1", input1);
        insertResult1.IsRight.ShouldBeTrue();

        var insertResult2 = await _sut.SetAsync(key, "field2", input2);
        insertResult2.IsRight.ShouldBeTrue();

        var getResult1 = await _sut.GetAsync<TestModel>(key, "field1");
        getResult1.IsRight.ShouldBeTrue();
        getResult1.OnRight(o =>
        {
            o.IsSome.ShouldBeTrue();
            o.OnSome(v => v.ShouldBeEquivalentTo(input1));
        });

        var getResult2 = await _sut.GetAsync<TestModel>(key, "field2");
        getResult2.IsRight.ShouldBeTrue();
        getResult2.OnRight(o =>
        {
            o.IsSome.ShouldBeTrue();
            o.OnSome(v => v.ShouldBeEquivalentTo(input2));
        });

        var getResult = await _sut.GetAsync<TestModel>(key, "field1", "field2");
        getResult.IsRight.ShouldBeTrue();
        getResult.OnRight(o => 
            o.ShouldBeEquivalentTo(new[] 
            {
                Option<TestModel>.Some(input1),
                Option<TestModel>.Some(input2)
            }));

        var getResultWithMisingField = await _sut.GetAsync<TestModel>(key, "field1", "field3");
        getResultWithMisingField.IsRight.ShouldBeTrue();
        getResultWithMisingField.OnRight(o => 
            o.ShouldBeEquivalentTo(new[] 
            {
                Option<TestModel>.Some(input1),
                Option<TestModel>.None()
            }));
    }
}
