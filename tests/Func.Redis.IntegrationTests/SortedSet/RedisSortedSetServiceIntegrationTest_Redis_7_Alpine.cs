namespace Func.Redis.IntegrationTests.SortedSet;

internal class RedisSortedSetServiceIntegrationTest_Redis_7_Alpine : RedisSortedSetServiceIntegrationTest
{
    public RedisSortedSetServiceIntegrationTest_Redis_7_Alpine() : base("redis:7-alpine") { }
}
