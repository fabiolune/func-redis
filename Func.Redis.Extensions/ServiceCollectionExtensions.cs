using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sport.Configuration.Extensions;
using Sport.Functional.Extensions;
using Sport.Redis.Models;
using Sport.Redis.SerDes;
using Sport.Redis.SerDes.Json;
using System.Reflection;

namespace Sport.Redis.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config, RedisCapabilities capabilities, params Assembly[] assemblies) =>
        (services, config)
            .Tee(t => t.services
                .AddSingleton<IConnectionMultiplexerProvider, ConnectionMultiplexerProvider>()
                .AddSingleton<ISourcesProvider, RedisSourcesProvider>()
            )
            .Tee(t => t.services.AddSingleton(t
                        .config
                        .GetRequiredConfiguration<RedisConfiguration>()
                        .MakeOption(c => string.IsNullOrWhiteSpace(c.ConnectionString))
                        .IfNone(() => throw new Exception($"{nameof(RedisConfiguration)}: {nameof(RedisConfiguration.ConnectionString)} is invalid"))))
            .TeeWhen(t => t.services.AddSingleton(t
                    .config.GetConfiguration<RedisKeyConfiguration>()
                    .OrElse(new RedisKeyConfiguration())),
                () => capabilities.HasFlag(RedisCapabilities.Keys) || capabilities.HasFlag(RedisCapabilities.HashSet))
            .TeeWhen(t => t.services
                .AddSingleton<IRedisService, RedisKeyService>(),
                () => capabilities.HasFlag(RedisCapabilities.Keys))
            .TeeWhen(t => t.services
                .AddSingleton<IRedisHashSetService, RedisHashSetService>(),
                () => capabilities.HasFlag(RedisCapabilities.HashSet))
            .TeeWhen(t =>
                    t.services
                        .AddSingleton<IRedisPublisherService, RedisPublisherService>(),
                () => capabilities.HasFlag(RedisCapabilities.Publisher))
            .TeeWhen(t => t.services
                    .Tee(s => s
                    .Scan(selector => selector
                        .FromAssemblies(assemblies)
                        .AddClasses(c => c.AssignableTo<IRedisSubscriber>())
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime())), () => capabilities.HasFlag(RedisCapabilities.Subscriber))
            .Map(t => t.services);

    public static IServiceCollection AddSystemJsonRedisSerDes(this IServiceCollection services) =>
        services
            .AddSingleton<IRedisSerDes, SystemJsonRedisSerDes>();

    public static IServiceCollection AddRedisSerDes<T>(this IServiceCollection services) where T : IRedisSerDes =>
        services
            .AddSingleton(typeof(IRedisSerDes), typeof(T));
}