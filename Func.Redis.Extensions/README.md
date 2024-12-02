# Functional Redis Extensions

[![NuGet](https://img.shields.io/nuget/v/func-redis-extensions)](https://www.nuget.org/packages/func-redis-extensions/)
[![NuGet](https://img.shields.io/nuget/dt/func-redis-extensions)](https://www.nuget.org/packages/func-redis-extensions/)

The `Func.Redis.Extensions` project contains additional functionality and utilities that extend the core capabilities of the Func.Redis project allowing for a simplified setup of the dependency injection container.

The main functionalities are the `IServiceCollection` `AddRedis` extension method that accepts the following parameters:

- `IConfiguration` __config__ to retrieve the `RedisConfiguration` object from any supported provider (json file, env variables, ...) and an optional `RedisKeyConfiguration` that allows prefixing keys
- `RedisCapabilities` __capabilities__: to select the functionality to be enabled (the enum supports bitwise operations)
- `bool` __addLogging__ to enable predefined logging functionalities
- `Assembly[]` assemblies to scan for specific instances of `IRedisSubscriber` (when `RedisCapabilities.Subscribe` is enabled)

The extension allows to specify the right implementation of `IRedisSerDes` to deal with serialization and deserialization
