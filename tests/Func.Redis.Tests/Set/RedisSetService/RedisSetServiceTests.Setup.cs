using Func.Redis.SerDes;

namespace Func.Redis.Tests.RedisSetService;
internal partial class RedisSetServiceTests
{
    private Redis.Set.RedisSetService _sut;
    private ISourcesProvider _mockSourcesProvider;
    private IRedisSerDes _mockSerDes;
    private IDatabase _mockDb;

    [SetUp]
    public void SetUp()
    {
        _mockSerDes = Substitute.For<IRedisSerDes>();
        _mockSourcesProvider = Substitute.For<ISourcesProvider>();
        _mockDb = Substitute.For<IDatabase>();
        _mockSourcesProvider.GetDatabase().Returns(_mockDb);
        _sut = new Redis.Set.RedisSetService(_mockSourcesProvider, _mockSerDes);
    }
}
