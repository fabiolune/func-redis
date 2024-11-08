> This documentation is in line with the active development, hence should be considered work in progress. To check the documentation for the latest stable version please visit [https://fabiolune.github.io/func-redis/](https://fabiolune.github.io/func-redis/)

# Functional Redis

![GitHub CI](https://github.com/fabiolune/func-redis/actions/workflows/main.yaml/badge.svg)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=fabiolune_func-redis&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=fabiolune_func-redis)
[![codecov](https://codecov.io/gh/fabiolune/func-redis/graph/badge.svg?token=EBG533UNGE)](https://codecov.io/gh/fabiolune/func-redis)
[![NuGet](https://img.shields.io/nuget/v/func-redis)](https://www.nuget.org/packages/func-redis/)
[![NuGet](https://img.shields.io/nuget/dt/func-redis)](https://www.nuget.org/packages/func-redis/)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Ffabiolune%2Ffunc-redis%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/fabiolune/func-redis/main)

This repo contains components aimed to simplify the adoption of Redis in `dotnet` using a Functional Programming-first approach (thanks to Franco Melandri's [tiny-fp](https://github.com/FrancoMelandri/tiny-fp)).

The library includes functionalities for:

- keys management (get, set, del) using `IRedisKeyService`
- hash sets using `IRedisHashSetService`
- pub/sub using `IRedisPublisherService` and `IRedisSubscriber`

To register the required components, use:

``` C#
...
    services
        .AddRedis<TypeOfRedisSerDes>(capabilities)
        ...
```

where `capabilities` is a bitwise combination of RedisCapabilities:

- `RedisCapabilities.Keys` enables keys management
- `RedisCapabilities.HashSet` enables hash sets management
- `RedisCapabilities.Publisher` enables publish management
- `RedisCapabilities.Subscriber` enables subscribe management (the `AddRedis` service collection extension requires an array of assemblies to enable scanning of `IRedisSubscriber` implementations).
