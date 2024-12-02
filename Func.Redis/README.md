# Functional Redis

[![NuGet](https://img.shields.io/nuget/v/func-redis)](https://www.nuget.org/packages/func-redis/)
[![NuGet](https://img.shields.io/nuget/dt/func-redis)](https://www.nuget.org/packages/func-redis/)

The purpose of the `Func.Redis` library is to abstract and manage Redis operations.

The library includes functionalities for:

- keys management (get, set, del) using `IRedisKeyService`
- hash sets using `IRedisHashSetService`
- sets using `IRedisSetService`
- lists using `IRedisListService`
- pub/sub using `IRedisPublisherService` and `IRedisSubscriber`
- generic interaction with Redis for all the use cases not covered by the above implementations using `IRedisService`