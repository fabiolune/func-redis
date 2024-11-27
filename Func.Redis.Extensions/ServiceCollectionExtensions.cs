using Func.Redis.HashSet;
using Func.Redis.Key;
using Func.Redis.List;
using Func.Redis.Models;
using Func.Redis.Publisher;
using Func.Redis.SerDes;
using Func.Redis.SerDes.Json;
using Func.Redis.Set;
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

    public static IServiceCollection AddRedis<T>(this IServiceCollection services, IConfiguration config, RedisCapabilities capabilities, params Assembly[] assemblies) where T : IRedisSerDes =>
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
            .AddRedisSerDes<T>();

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
                () => capabilities.HasFlag(RedisCapabilities.Keys)
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
                () => capabilities.HasFlag(RedisCapabilities.Keys))
            .TeeWhen(
                s => s.AddSingleton<IRedisHashSetService, RedisHashSetService>(),
                () => capabilities.HasFlag(RedisCapabilities.HashSet))
            .TeeWhen(
                s => s.AddSingleton<IRedisPublisherService, RedisPublisherService>(),
                () => capabilities.HasFlag(RedisCapabilities.Publisher))
            .TeeWhen(
                s => s.AddSingleton<IRedisSetService, RedisSetService>(),
                () => capabilities.HasFlag(RedisCapabilities.Set))
            .TeeWhen(
                s => s.AddSingleton<IRedisListService, RedisListService>(),
                () => capabilities.HasFlag(RedisCapabilities.List))
            .TeeWhen(
                s => s.Scan(selector => selector
                    .FromAssemblies(assemblies)
                    .AddClasses(c => c.AssignableTo<IRedisSubscriber>())
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime()),
                () => capabilities.HasFlag(RedisCapabilities.Subscriber));

    public static IServiceCollection AddSystemJsonRedisSerDes(this IServiceCollection services) =>
        services
            .AddSingleton<IRedisSerDes, SystemJsonRedisSerDes>();

    public static IServiceCollection AddRedisSerDes<T>(this IServiceCollection services) where T : IRedisSerDes =>
        services
            .AddSingleton(typeof(IRedisSerDes), typeof(T));
}