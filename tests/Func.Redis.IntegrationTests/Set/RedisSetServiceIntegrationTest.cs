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

        var addResult = await _sut.AddAsync(key, input);
        addResult.IsRight.ShouldBeTrue();

        var getAllResult = await _sut.GetAllAsync<TestModel>(key);

        getAllResult.IsRight.ShouldBeTrue();
        getAllResult.OnRight(o =>
        {
            o.ShouldContain(input.ToOption());
            o.Length.ShouldBe(1);
        });

        var sizeResult = await _sut.SizeAsync(key);
        sizeResult.IsRight.ShouldBeTrue();
        sizeResult.OnRight(o => o.ShouldBe(1));

        var input2 = new TestModel
        {
            Id = Guid.NewGuid()
        };

        addResult = await _sut.AddAsync(key, input2);
        addResult.IsRight.ShouldBeTrue();

        getAllResult = await _sut.GetAllAsync<TestModel>(key);
        getAllResult.IsRight.ShouldBeTrue();
        getAllResult.OnRight(o =>
        {
            o.Length.ShouldBe(2);
            o.ShouldContain(input.ToOption());
            o.ShouldContain(input2.ToOption());
        });

        sizeResult = await _sut.SizeAsync(key);
        sizeResult.IsRight.ShouldBeTrue();
        sizeResult.OnRight(o => o.ShouldBe(2));
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

        addResult1.IsRight.ShouldBeTrue();

        var addResult2 = await _sut.AddAsync(key2, input3)
            .BindAsync(_ => _sut.AddAsync(key2, input4))
            .BindAsync(_ => _sut.AddAsync(key2, input5));

        addResult2.IsRight.ShouldBeTrue();

        var intersectResult = await _sut.IntersectAsync<TestModel>(key1, key2);

        intersectResult.IsRight.ShouldBeTrue();
        intersectResult.OnRight(o =>
        {
            o.ShouldContain(input3);
            o.ShouldContain(input4);
            o.Length.ShouldBe(2);
        });

        var unionResult = await _sut.UnionAsync<TestModel>(key1, key2);

        unionResult.IsRight.ShouldBeTrue();
        unionResult.OnRight(o =>
        {
            o.ShouldContain(input1);
            o.ShouldContain(input2);
            o.ShouldContain(input3);
            o.ShouldContain(input4);
            o.ShouldContain(input5);
            o.Length.ShouldBe(5);
        });

        var differenceResult = await _sut.DifferenceAsync<TestModel>(key1, key2);
        differenceResult.IsRight.ShouldBeTrue();
        differenceResult.OnRight(o =>
        {
            o.ShouldContain(input1);
            o.ShouldContain(input2);
            o.Length.ShouldBe(2);
        });
    }
}
