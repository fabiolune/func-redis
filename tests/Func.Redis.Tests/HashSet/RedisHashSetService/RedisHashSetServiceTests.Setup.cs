﻿using Func.Redis.SerDes;

namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{
    private Redis.HashSet.RedisHashSetService _sut;
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
        _sut = new Redis.HashSet.RedisHashSetService(_mockProvider, _mockSerDes);
    }
}