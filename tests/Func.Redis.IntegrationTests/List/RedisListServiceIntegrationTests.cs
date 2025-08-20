using Func.Redis.List;

namespace Func.Redis.IntegrationTests.List;
internal abstract class RedisListServiceIntegrationTests(string redisImage) : RedisIntegrationTestBase(redisImage)
{
    private RedisListService _sut;

    [OneTimeSetUp]
    public override async Task OneTimeSetUp()
    {
        await base.OneTimeSetUp();

        _sut = _provider.Map(sp => new RedisListService(sp, new SystemJsonRedisSerDes()));
    }

    [Test]
    public async Task WhenDataArePresent_GetShouldReturnRightValue()
    {
        const string key = nameof(WhenDataArePresent_GetShouldReturnRightValue);
        var value = new TestModel();

        var getResult = await _sut.GetAsync<TestModel>(key, 0);

        getResult.IsRight.ShouldBeTrue();
        getResult.OnRight(v => v.IsNone.ShouldBeTrue());

        await _sut.AppendAsync(key, value);

        getResult = await _sut.GetAsync<TestModel>(key, 0);

        getResult.IsRight.ShouldBeTrue();
        getResult.OnRight(v =>
        {
            v.IsSome.ShouldBeTrue();
            v.OnSome(s => s.ShouldBe(value));
        });

        getResult = await _sut.GetAsync<TestModel>(key, 10);
        getResult.IsRight.ShouldBeTrue();
        getResult.OnRight(v => v.IsNone.ShouldBeTrue());

        var value2 = new TestModel();
        await _sut.PrependAsync(key, value2);

        var sizeResult = await _sut.SizeAsync(key);

        sizeResult.IsRight.ShouldBeTrue();
        sizeResult.OnRight(s => s.ShouldBe(2));

        getResult = await _sut.GetAsync<TestModel>(key, 0);
        getResult.IsRight.ShouldBeTrue();
        getResult.OnRight(v =>
        {
            v.IsSome.ShouldBeTrue();
            v.OnSome(s => s.ShouldBe(value2));
        });

        var value3 = new TestModel();
        await _sut.AppendAsync(key, value3);

        var valuesResult = await _sut.GetAsync<TestModel>(key, 0, 2);

        valuesResult.IsRight.ShouldBeTrue();
        valuesResult.OnRight(v => v.Filter().Tee(f =>
        {
            f.Count().ShouldBe(3);
            f.ShouldContain(value2);
            f.ShouldContain(value);
            f.ShouldContain(value3);
        }));

        sizeResult = await _sut.SizeAsync(key);

        sizeResult.IsRight.ShouldBeTrue();
        sizeResult.OnRight(s => s.ShouldBe(3));

        var popResult = await _sut.PopAsync<TestModel>(key);

        popResult.IsRight.ShouldBeTrue();
        popResult.OnRight(v =>
        {
            v.IsSome.ShouldBeTrue();
            v.OnSome(s => s.ShouldBe(value3));
        });

        sizeResult = await _sut.SizeAsync(key);

        sizeResult.IsRight.ShouldBeTrue();
        sizeResult.OnRight(s => s.ShouldBe(2));

        var shiftResult = await _sut.ShiftAsync<TestModel>(key);

        shiftResult.IsRight.ShouldBeTrue();
        shiftResult.OnRight(v =>
        {
            v.IsSome.ShouldBeTrue();
            v.OnSome(s => s.ShouldBe(value2));
        });
    }
}
