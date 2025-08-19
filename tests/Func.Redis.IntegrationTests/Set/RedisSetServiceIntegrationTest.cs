using Func.Redis.Set;

namespace Func.Redis.IntegrationTests.Set;

internal abstract class RedisSetServiceIntegrationTest(string redisImage) : RedisIntegrationTestBase(redisImage)
{
    private RedisSetService _sut;

    [OneTimeSetUp]
    public override async Task OneTimeSetUp()
    {
        await base.OneTimeSetUp();

        _sut = _provider.Map(sp => new RedisSetService(sp, new SystemJsonRedisSerDes()));
    }

    [Test]
    public async Task WhenDataAreNotPresent_AddShouldReturnRightUnit()
    {
        var addResult = await _sut.AddAsync("some key", "some value");

        addResult.IsRight.Should().BeTrue();
    }

    [Test]
    public async Task WhenDataAreSuccessfullyAdded_TheyShouldBeSuccessfullyRetrieved()
    {
        var key = nameof(WhenDataAreSuccessfullyAdded_TheyShouldBeSuccessfullyRetrieved);

        var input = new TestModel
        {
            Id = Guid.NewGuid()
        };

        var addResult = await _sut.AddAsync(key, input);
        addResult.IsRight.Should().BeTrue();

        var getAllResult = await _sut.GetAllAsync<TestModel>(key);

        getAllResult.IsRight.Should().BeTrue();
        getAllResult.OnRight(o => o.Should().BeEquivalentTo([input.ToOption()]));

        var sizeResult = await _sut.SizeAsync(key);

        sizeResult.IsRight.Should().BeTrue();
        sizeResult.OnRight(o => o.Should().Be(1));

        var input2 = new TestModel
        {
            Id = Guid.NewGuid()
        };

        addResult = await _sut.AddAsync(key, input2);
        addResult.IsRight.Should().BeTrue();

        getAllResult = await _sut.GetAllAsync<TestModel>(key);

        getAllResult.IsRight.Should().BeTrue();
        getAllResult.OnRight(o => o.Should().BeEquivalentTo([input.ToOption(), input2.ToOption()]));

        sizeResult = await _sut.SizeAsync(key);

        sizeResult.IsRight.Should().BeTrue();
        sizeResult.OnRight(o => o.Should().Be(2));
    }

    [Test]
    public async Task WhenDataArePresentInDifferentSets_CombineOperationShouldReturnExpectedResults()
    {
        var key1 = $"{nameof(WhenDataArePresentInDifferentSets_CombineOperationShouldReturnExpectedResults)}_1";
        var key2 = $"{nameof(WhenDataArePresentInDifferentSets_CombineOperationShouldReturnExpectedResults)}_2";

        var input1 = new TestModel
        {
            Id = Guid.NewGuid()
        };
        var input2 = new TestModel
        {
            Id = Guid.NewGuid()
        };
        var input3 = new TestModel
        {
            Id = Guid.NewGuid()
        };
        var input4 = new TestModel
        {
            Id = Guid.NewGuid()
        };
        var input5 = new TestModel
        {
            Id = Guid.NewGuid()
        };

        var addResult1 = await _sut.AddAsync(key1, input1)
            .BindAsync(_ => _sut.AddAsync(key1, input2))
            .BindAsync(_ => _sut.AddAsync(key1, input3))
            .BindAsync(_ => _sut.AddAsync(key1, input4));

        addResult1.IsRight.Should().BeTrue();

        var addResult2 = await _sut.AddAsync(key2, input3)
            .BindAsync(_ => _sut.AddAsync(key2, input4))
            .BindAsync(_ => _sut.AddAsync(key2, input5));

        addResult2.IsRight.Should().BeTrue();

        var intersectResult = await _sut.IntersectAsync<TestModel>(key1, key2);

        intersectResult.IsRight.Should().BeTrue();
        intersectResult.OnRight(o => o.Should().BeEquivalentTo([input3, input4]));

        var unionResult = await _sut.UnionAsync<TestModel>(key1, key2);

        unionResult.IsRight.Should().BeTrue();
        unionResult.OnRight(o => o.Should().BeEquivalentTo([input1, input2, input3, input4, input5]));

        var differenceResult = await _sut.DifferenceAsync<TestModel>(key1, key2);

        differenceResult.IsRight.Should().BeTrue();
        differenceResult.OnRight(o => o.Should().BeEquivalentTo([input1, input2]));
    }
}
