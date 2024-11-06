# Redis Data Components

This repo contains components aimed to simplify the adoption of Redis in `dotnet` using a Functional Programming first approach.

The library includes functionalities to work with:
- keys management (get, set, del) using `IRedisKeyService`
- hash sets using `IRedisHashSetService`
- pub/sub using `IRedisPublisherService` and `IRedisSubscriber`

To register the required components use:

``` C#
...
    services
        .AddRedis(capabilities)
...
```

where `capabilities` is a bitwise combination of RedisCapabilities:
- `RedisCapabilities.Keys` enables keys management
- `RedisCapabilities.HashSet` enables hash sets management
- `RedisCapabilities.Publisher` enables publish management
- `RedisCapabilities.Subscriber` enables subscribe management (the `AddRedis` service collecction extensions requires an array of assemblies to enable scanning of `IRedisSubscriber` implementations).
