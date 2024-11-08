# Functional Redis Extensions

![GitHub CI](https://github.com/fabiolune/func-redis/actions/workflows/main.yaml/badge.svg)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=fabiolune_func-redis&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=fabiolune_func-redis)
[![codecov](https://codecov.io/gh/fabiolune/func-redis/graph/badge.svg?token=EBG533UNGE)](https://codecov.io/gh/fabiolune/func-redis)
[![NuGet](https://img.shields.io/nuget/v/func-redis-extensions)](https://www.nuget.org/packages/func-redis-extensions/)
[![NuGet](https://img.shields.io/nuget/dt/func-redis-extensions)](https://www.nuget.org/packages/func-redis-extensions/)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Ffabiolune%2Ffunc-redis%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/fabiolune/func-redis/main)

The Func.Redis.Extensions project contains additional functionality and utilities that extend the core capabilities of the Func.Redis project.

These extensions are designed to provide more specialized or advanced features that complement the basic Redis operations provided by the core Func.Redis services.

Here’s a breakdown of the key components and their purposes:

- __Enhanced Functionality__: The extensions provide additional features that go beyond the basic Redis operations, enabling more advanced use cases and improving developer productivity.
- __Convenience__: By offering extension methods and utilities, the code in this folder makes it easier to perform common tasks and handle complex scenarios with less boilerplate code.
- __Flexibility__: Custom serializers and configuration enhancements allow developers to tailor Redis interactions to their specific needs, supporting a wider range of data formats and operational requirements.
- __Optimization__: Utility functions can help optimize performance by providing efficient ways to handle batch operations, transactions, and other advanced patterns.

## How to Use the Extensions

### Installation

To install the Func.Redis.Extensions package, use

```shell
dotnet add package func-redis-extensions
```