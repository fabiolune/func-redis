> This documentation is in line with the active development, hence should be considered work in progress. To check the documentation for the latest stable version please visit [https://fabiolune.github.io/func-redis/](https://fabiolune.github.io/func-redis/)

# Functional Redis

This repo contains components aimed to simplify the adoption of Redis in `dotnet` using a Functional Programming first approach (thanks to Franco Melandri's [tiny-fp](https://github.com/FrancoMelandri/tiny-fp)).

The library includes functionalities to work with:

- keys management (get, set, del) using `IRedisKeyService`
- hash sets using `IRedisHashSetService`
- pub/sub using `IRedisPublisherService` and `IRedisSubscriber`

To register the required components use:

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
- `RedisCapabilities.Subscriber` enables subscribe management (the `AddRedis` service collection extensions requires an array of assemblies to enable scanning of `IRedisSubscriber` implementations).
