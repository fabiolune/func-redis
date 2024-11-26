using Func.Redis.SerDes;

namespace Func.Redis.Tests.RedisKeyService;

public partial class RedisKeyServiceTests
{
    private Redis.Key.RedisKeyService _sut;
    private ISourcesProvider _mockSourcesProvider;
    private IDatabase _mockDb;
    private IRedisSerDes _mockSerDes;
    private IServer _mockServer1;
    private IServer _mockServer2;

    [SetUp]
    public void Setup()
    {
        _mockSerDes = Substitute.For<IRedisSerDes>();
        _mockDb = Substitute.For<IDatabase>();
        _mockSourcesProvider = Substitute.For<ISourcesProvider>();
        _mockSourcesProvider.GetDatabase().Returns(_mockDb);
        _mockServer1 = Substitute.For<IServer>();
        _mockServer2 = Substitute.For<IServer>();
        _mockSourcesProvider.GetServers().Returns([_mockServer1, _mockServer2]);
        _sut = new Redis.Key.RedisKeyService(_mockSourcesProvider, _mockSerDes);
    }
}