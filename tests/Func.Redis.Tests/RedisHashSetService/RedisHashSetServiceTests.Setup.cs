using Func.Redis.SerDes;

namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{
    private Redis.RedisHashSetService _sut;
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
    }

    private record TestData(string Id);
}