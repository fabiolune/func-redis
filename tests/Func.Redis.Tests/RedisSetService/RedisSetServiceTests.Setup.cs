using Func.Redis.SerDes;

namespace Func.Redis.Tests.RedisSetService;
internal partial class RedisSetServiceTests
{
    private Set.RedisSetService _sut;
    private ISourcesProvider _mockSourcesProvider;
    private IRedisSerDes _mockSerDes;
    private IDatabase _mockDb;

    private record TestData(string Id);

    [SetUp]
    public void SetUp()
    {
        _mockSerDes = Substitute.For<IRedisSerDes>();
        _mockSourcesProvider = Substitute.For<ISourcesProvider>();
        _mockDb = Substitute.For<IDatabase>();
        _mockSourcesProvider.GetDatabase().Returns(_mockDb);
        _sut = new Redis.RedisSetService(_mockSourcesProvider, _mockSerDes);
    }
}
