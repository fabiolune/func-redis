namespace Func.Redis.IntegrationTests.HashSet;

internal class RedisHashSetServiceIntegrationTest_Redis_7_Alpine : RedisHashSetServiceIntegrationTest
{
    public RedisHashSetServiceIntegrationTest_Redis_7_Alpine() : base("redis:7-alpine") { }
}
