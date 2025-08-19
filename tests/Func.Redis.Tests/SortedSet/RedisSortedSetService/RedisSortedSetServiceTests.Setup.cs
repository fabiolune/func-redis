using Func.Redis.SerDes;

namespace Func.Redis.Tests.SortedSet.RedisSortedSetService;
internal partial class RedisSortedSetServiceTests
{
    private Redis.SortedSet.RedisSortedSetService _sut;

    private ISourcesProvider _mockProvider;
    private IDatabase _mockDb;
    private IRedisSerDes _mockSerDes;

    [SetUp]
    public void SetUp()
    {
        _mockSerDes = Substitute.For<IRedisSerDes>();
        _mockDb = Substitute.For<IDatabase>();
        _mockProvider = Substitute.For<ISourcesProvider>();
        _mockProvider.GetDatabase().Returns(_mockDb);
        _sut = new Redis.SortedSet.RedisSortedSetService(_mockProvider, _mockSerDes);
    }
}

