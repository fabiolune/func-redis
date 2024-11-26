using Func.Redis.SerDes;

namespace Func.Redis.Tests.RedisListService;
internal partial class RedisListServiceTests
{
    private Redis.List.RedisListService _sut;
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

        _sut = new Redis.List.RedisListService(_mockProvider, _mockSerDes);
    }
}