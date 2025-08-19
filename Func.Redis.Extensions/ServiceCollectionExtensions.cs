using Func.Redis.HashSet;
using Func.Redis.Key;
using Func.Redis.List;
using Func.Redis.Models;
using Func.Redis.Publisher;
using Func.Redis.SerDes;
using Func.Redis.SerDes.Json;
using Func.Redis.Set;
using Func.Redis.SortedSet;
using Func.Redis.Subscriber;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TinyFp;
using TinyFp.Extensions;

namespace Func.Redis.Extensions;

public static class ServiceCollectionExtensions
{
    private static readonly Error MissingConfiguration = Error.New($"Missing {nameof(RedisConfiguration)}");
    private static readonly Error InvalidConnectionString = Error.New($"{nameof(RedisConfiguration)}: {nameof(RedisConfiguration.ConnectionString)} is invalid");

    /// <summary>
    /// Add Redis services to the service collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services"></param>
    /// <param name="config">Configuration provider to retrieve configuration parametes for <see cref="RedisConfiguration"/> and <see cref="RedisKeyConfiguration"/></param>
    /// <param name="capabilities">Specify capabilities to be enabled</param>
    /// <param name="addLogging">Enable/disable loggind</param>
    /// <param name="assemblies">Specifiy assemblies to be scanned for <see cref="IRedisSubscriber"/> implementations</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static IServiceCollection AddRedis<T>(
        this IServiceCollection services,
        IConfiguration config,
        RedisCapabilities capabilities,
        bool addLogging = false,
        params Assembly[] assemblies) where T : IRedisSerDes =>
        (services, config)
            .Tee(t =>
                t.config
                    .GetSection(nameof(RedisConfiguration))
                    .Get<RedisConfiguration>()
                    .ToOption()
                    .Map(r => r!)
                    .ToEither(MissingConfiguration)
                    .Bind(rc => rc.ToOption(r => string.IsNullOrWhiteSpace(r.ConnectionString)).ToEither(InvalidConnectionString))
                    .Match(rc => t.services.AddSingleton(rc), e => throw new KeyNotFoundException(e.Message)))
            .Tee(t => t.services
                .AddSingleton<IConnectionMultiplexerProvider, ConnectionMultiplexerProvider>()
                .AddSingleton<ISourcesProvider, RedisSourcesProvider>())
            .Map(t => (t.services, t.config.GetSection(nameof(RedisKeyConfiguration))))
            .Map(t => t.Item2.Get<RedisKeyConfiguration>().ToOption()
                .Match(
                    sec => Either<IServiceCollection, (IServiceCollection, RedisKeyConfiguration)>.Right((t.services, sec!)),
                    () => Either<IServiceCollection, (IServiceCollection, RedisKeyConfiguration)>.Left(t.services)))
            .Match(
                t => (t.Item1, t.Item2).Map(t => AddKeyTransformingRedis(t.Item1, t.Item2.GetKeyMapper(), t.Item2.GetInverseKeyMapper(), capabilities, assemblies)),
                s => s.InternalAddRedis(capabilities, assemblies))
            .AddRedisSerDes<T>()
            .TeeWhen(s => s.AddLoggingRedis(capabilities), () => addLogging);

    private static IServiceCollection AddKeyTransformingRedis(
        IServiceCollection services,
        Func<string, string> keyMapper,
        Func<string, string> inverseKeyMapper,
        RedisCapabilities capabilities,
        params Assembly[] assemblies) =>
        services
            .InternalAddRedis(capabilities, assemblies)
            .TeeWhen(
                s => s.Decorate<IRedisKeyService>(ks => new KeyTransformerRedisKeyService(ks, keyMapper, inverseKeyMapper)),
                () => capabilities.HasFlag(RedisCapabilities.Key)
            )
            .TeeWhen(
                s => s.Decorate<IRedisHashSetService>(hs => new KeyTransformerRedisHashSetService(hs, keyMapper)),
                () => capabilities.HasFlag(RedisCapabilities.HashSet)
            )
            .TeeWhen(
                s => s.Decorate<IRedisSetService>(hs => new KeyTransformerRedisSetService(hs, keyMapper)),
                () => capabilities.HasFlag(RedisCapabilities.Set)
            )
            .TeeWhen(
                s => s.Decorate<IRedisSortedSetService>(hs => new KeyTransformerRedisSortedSetService(hs, keyMapper)),
                () => capabilities.HasFlag(RedisCapabilities.SortedSet)
            )
            .TeeWhen(
                s => s.Decorate<IRedisListService>(hs => new KeyTransformerRedisListService(hs, keyMapper)),
                () => capabilities.HasFlag(RedisCapabilities.List)
            );

    private static IServiceCollection InternalAddRedis(
        this IServiceCollection services,
        RedisCapabilities capabilities,
        params Assembly[] assemblies) =>
        services
            .TeeWhen(
                s => s.AddSingleton<IRedisService, RailwayRedisService>(),
                () => capabilities.HasFlag(RedisCapabilities.Generic))
            .TeeWhen(
                s => s.AddSingleton<IRedisKeyService, RedisKeyService>(),
                () => capabilities.HasFlag(RedisCapabilities.Key))
            .TeeWhen(
                s => s.AddSingleton<IRedisHashSetService, RedisHashSetService>(),
                () => capabilities.HasFlag(RedisCapabilities.HashSet))
            .TeeWhen(
                s => s.AddSingleton<IRedisPublisherService, RedisPublisherService>(),
                () => capabilities.HasFlag(RedisCapabilities.Publish))
            .TeeWhen(
                s => s.AddSingleton<IRedisSetService, RedisSetService>(),
                () => capabilities.HasFlag(RedisCapabilities.Set))
            .TeeWhen(
                s => s.AddSingleton<IRedisSortedSetService, RedisSortedSetService>(),
                () => capabilities.HasFlag(RedisCapabilities.SortedSet))
            .TeeWhen(
                s => s.AddSingleton<IRedisListService, RedisListService>(),
                () => capabilities.HasFlag(RedisCapabilities.List))
            .TeeWhen(
                s => s.Scan(selector =>
                    selector
                        .FromAssemblies(assemblies)
                        .AddClasses(c => c.AssignableTo<IRedisSubscriber>())
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime()),
                () => capabilities.HasFlag(RedisCapabilities.Subscribe));

    /// <summary>
    /// Add <see cref="SystemJsonRedisSerDes"/> Redis serializer/deserializer
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSystemJsonRedisSerDes(this IServiceCollection services) =>
        services
            .AddSingleton<IRedisSerDes, SystemJsonRedisSerDes>();

    /// <summary>
    /// Add a custom implementation of <see cref="IRedisSerDes"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRedisSerDes<T>(this IServiceCollection services) where T : IRedisSerDes =>
        services
            .AddSingleton(typeof(IRedisSerDes), typeof(T));

    private static IServiceCollection AddLoggingRedis(this IServiceCollection services, RedisCapabilities capabilities) =>
        services
            .TeeWhen(
                s => s.Decorate<IRedisKeyService, LoggingRedisKeyService>(),
                () => capabilities.HasFlag(RedisCapabilities.Key)
            )
            .TeeWhen(
                s => s.Decorate<IRedisHashSetService, LoggingRedisHashSetService>(),
                () => capabilities.HasFlag(RedisCapabilities.HashSet)
            )
            .TeeWhen(
                s => s.Decorate<IRedisSetService, LoggingRedisSetService>(),
                () => capabilities.HasFlag(RedisCapabilities.Set)
            )
            .TeeWhen(
                s => s.Decorate<IRedisSortedSetService, LoggingRedisSortedSetService>(),
                () => capabilities.HasFlag(RedisCapabilities.SortedSet)
            )
            .TeeWhen(
                s => s.Decorate<IRedisListService, LoggingRedisListService>(),
                () => capabilities.HasFlag(RedisCapabilities.List)
            )
            .TeeWhen(
                s => s.Decorate<IRedisPublisherService, LoggingRedisPublisherService>(),
                () => capabilities.HasFlag(RedisCapabilities.Publish)
            );

}