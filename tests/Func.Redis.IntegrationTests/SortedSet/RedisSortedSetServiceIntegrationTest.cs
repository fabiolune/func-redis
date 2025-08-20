using Func.Redis.SortedSet;

namespace Func.Redis.IntegrationTests.SortedSet;

internal abstract class RedisSortedSetServiceIntegrationTest(string redisImage) : RedisIntegrationTestBase(redisImage)
{
    private RedisSortedSetService _sut;

    [OneTimeSetUp]
    public override async Task OneTimeSetUp()
    {
        await base.OneTimeSetUp();

        _sut = _provider.Map(sp => new RedisSortedSetService(sp, new SystemJsonRedisSerDes()));
    }

    [Test]
    public async Task WhenDataAreNotPresent_AddShouldReturnRightUnit()
    {
        var addResult = await _sut.AddAsync("some key", "some value", 10);

        addResult.IsRight.ShouldBeTrue();
    }

    [Test]
    public async Task WhenDataAreSuccessfullyAdded_TheyShouldBeSuccessfullyRetrieved()
    {
        var key = nameof(WhenDataAreSuccessfullyAdded_TheyShouldBeSuccessfullyRetrieved);

        var input = new TestModel
        {
            Id = Guid.NewGuid()
        };

        var addResult = await _sut.AddAsync(key, input, 1);
        addResult.IsRight.ShouldBeTrue();

        var lengthResult = await _sut.LengthAsync(key);

        lengthResult.IsRight.ShouldBeTrue();
        lengthResult.OnRight(o => o.ShouldBe(1));

        var input2 = new TestModel
        {
            Id = Guid.NewGuid()
        };

        addResult = await _sut.AddAsync(key, input2, 10);
        addResult.IsRight.ShouldBeTrue();

        lengthResult = await _sut.LengthAsync(key);

        lengthResult.IsRight.ShouldBeTrue();
        lengthResult.OnRight(o => o.ShouldBe(2));

        var rangeResult = await _sut.RangeByScoreAsync<TestModel>(key, 1, 5);
        rangeResult.IsRight.ShouldBeTrue();
        rangeResult.OnRight(o => o.ShouldBeEquivalentTo(new[] { input }));

        rangeResult = await _sut.RangeByScoreAsync<TestModel>(key, 1, 10);
        rangeResult.IsRight.ShouldBeTrue();
        rangeResult.OnRight(o => o.ShouldBeEquivalentTo(new[] { input, input2 }));
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

        var addResult1 = await _sut.AddAsync(key1, input1, 1)
            .BindAsync(_ => _sut.AddAsync(key1, input2, 2))
            .BindAsync(_ => _sut.AddAsync(key1, input3, 3))
            .BindAsync(_ => _sut.AddAsync(key1, input4, 4));

        addResult1.IsRight.ShouldBeTrue();

        var addResult2 = await _sut.AddAsync(key2, input3, 10)
            .BindAsync(_ => _sut.AddAsync(key2, input4, 11))
            .BindAsync(_ => _sut.AddAsync(key2, input5, 12));

        addResult2.IsRight.ShouldBeTrue();

        var keys = new[] { key1, key2 };
        var intersectResult = await _sut.IntersectAsync<TestModel>(keys);

        intersectResult.IsRight.ShouldBeTrue();
        intersectResult.OnRight(o =>
        {
            o.Length.ShouldBe(2);
            o.ShouldContain(input3);
            o.ShouldContain(input4);
        });

        var unionResult = await _sut.UnionAsync<TestModel>(keys);

        unionResult.IsRight.ShouldBeTrue();
        unionResult.OnRight(o =>
        {
            o.Length.ShouldBe(5);
            o.ShouldContain(input1);
            o.ShouldContain(input2);
            o.ShouldContain(input3);
            o.ShouldContain(input4);
            o.ShouldContain(input5);
        });
    }
}
