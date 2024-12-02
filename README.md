> This documentation is in line with the active development, hence should be considered work in progress. To check the documentation for the latest stable version please visit [https://fabiolune.github.io/func-redis/](https://fabiolune.github.io/func-redis/)

# Functional Redis

![GitHub CI](https://github.com/fabiolune/func-redis/actions/workflows/main.yaml/badge.svg)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=fabiolune_func-redis&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=fabiolune_func-redis)
[![codecov](https://codecov.io/gh/fabiolune/func-redis/graph/badge.svg?token=EBG533UNGE)](https://codecov.io/gh/fabiolune/func-redis)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Ffabiolune%2Ffunc-redis%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/fabiolune/func-redis/main)

__func-redis__ contains components aimed to simplify the adoption of [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) using a Functional Programming-first approach (thanks to Franco Melandri's [tiny-fp](https://github.com/FrancoMelandri/tiny-fp)).

The design of `Func.Redis` emphasizes a robust and flexible functional programming approach. Below are the key aspects that highlight the purpose and benefits of using Func.Redis in your .NET applications:

- __Abstraction__: These classes and interfaces abstract the complexity of interacting with Redis, providing a clean and consistent API for the rest of the application.
- __Key Transformation__: The key transformation functions allow for flexible key management, enabling the modification of keys before they are used in Redis operations.
- __Error Handling__: The use of `Either<Error, Unit>` and `Option<T>` types from the `TinyFp` library helps in handling errors and optional values in a functional programming style.
- __Asynchronous Operations__: The inclusion of asynchronous methods ensures that the application can perform Redis operations without blocking the main thread, improving performance and scalability.

## Usage

### Installation

```shell
dotnet add package func-redis
```

To include the functionalities to setup the native dependency injection container you will need to install the associated extensions:

```shell
dotnet add package func-redis-extensions
```

### Example

```csharp
using TinyFp;
using Func.Redis.Extensions;
using Func.Redis;
using Microsoft.Extensions.DependencyInjection;
using Func.Redis.SerDes.Json;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

// Configure services 
var config = new ConfigurationBuilder()
    .AddJsonFile("AppSettings.json", optional: false)
    .Build();

var services = new ServiceCollection();
// Use func-redis-extensions
services
  .AddRedis<SystemJsonRedisSerDes>(config, RedisCapabilities.Keys);

var serviceProvider = services.BuildServiceProvider();

// Resolve the Redis service 
var redisService = serviceProvider.GetRequiredService<IRedisKeyService>();

// Save and retrieve data
var result = await redisService
  .SetAsync("key", new TestData(42))
  .BindAsync(_ => redisService.GetAsync<TestData>("key"));

result
    .OnRight(o => o
        .OnSome(td => Console.WriteLine($"data found: {JsonSerializer.Serialize(td)}"))
        .OnNone(() => Console.WriteLine("data not found")))
    .OnLeft(e => Console.WriteLine($"Returned error {e.Message}"));

internal record TestData(int Id);
```
