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

        getResult.IsRight.Should().BeTrue();
        getResult.OnRight(v => v.IsNone.Should().BeTrue());

        await _sut.AppendAsync(key, value);

        getResult = await _sut.GetAsync<TestModel>(key, 0);

        getResult.IsRight.Should().BeTrue();
        getResult.OnRight(v =>
        {
            v.IsSome.Should().BeTrue();
            v.OnSome(s => s.Should().Be(value));
        });

        var value2 = new TestModel();
        await _sut.PrependAsync(key, value2);

        var sizeResult = await _sut.SizeAsync(key);

        sizeResult.IsRight.Should().BeTrue();
        sizeResult.OnRight(s => s.Should().Be(2));

        getResult = await _sut.GetAsync<TestModel>(key, 0);
        getResult.IsRight.Should().BeTrue();
        getResult.OnRight(v =>
        {
            v.IsSome.Should().BeTrue();
            v.OnSome(s => s.Should().Be(value2));
        });

        var value3 = new TestModel();
        await _sut.AppendAsync(key, value3);

        var valuesResult = await _sut.GetAsync<TestModel>(key, 0, 2);

        valuesResult.IsRight.Should().BeTrue();
        valuesResult.OnRight(v => v.Filter().Should().BeEquivalentTo(new[] { value2, value, value3 }));

        sizeResult = await _sut.SizeAsync(key);

        sizeResult.IsRight.Should().BeTrue();
        sizeResult.OnRight(s => s.Should().Be(3));

        var popResult = await _sut.PopAsync<TestModel>(key);

        popResult.IsRight.Should().BeTrue();
        popResult.OnRight(v =>
        {
            v.IsSome.Should().BeTrue();
            v.OnSome(s => s.Should().Be(value3));
        });

        sizeResult = await _sut.SizeAsync(key);

        sizeResult.IsRight.Should().BeTrue();
        sizeResult.OnRight(s => s.Should().Be(2));

        var shiftResult = await _sut.ShiftAsync<TestModel>(key);

        shiftResult.IsRight.Should().BeTrue();
        shiftResult.OnRight(v =>
        {
            v.IsSome.Should().BeTrue();
            v.OnSome(s => s.Should().Be(value2));
        });
    }
}
