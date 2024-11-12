namespace Func.Redis.IntegrationTests.Set;

internal class RedisSetServiceIntegrationTest_Redis_7_Alpine : RedisSetServiceIntegrationTest
{
    public RedisSetServiceIntegrationTest_Redis_7_Alpine() : base("redis:7-alpine")
    {
    }
}
