using FluentAssertions;
using Func.Redis.HashSet;
using Func.Redis.Key;
using Func.Redis.Models;
using Func.Redis.Publisher;
using Func.Redis.SerDes;
using Func.Redis.SerDes.Json;
using Func.Redis.Set;
using Func.Redis.Subscriber;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using StackExchange.Redis;
using System.Reflection;
using TinyFp;

namespace Func.Redis.Extensions.Unit.Tests;

public class ServiceCollectionExtensionsTests
{
    internal class StubSerdes : IRedisSerDes
    {
        public Option<T> Deserialize<T>(RedisValue value) => throw new NotImplementedException();
        public Option<T[]> Deserialize<T>(RedisValue[] values) => throw new NotImplementedException();
        public Option<(string, T)[]> Deserialize<T>(HashEntry[] entries) => throw new NotImplementedException();
        public Option<object> Deserialize(RedisValue value, Type type) => throw new NotImplementedException();
        public RedisValue Serialize<T>(T value) => throw new NotImplementedException();
    }

    internal class TestRedisSubscriber : IRedisSubscriber
    {
        public (string, Action<RedisChannel, RedisValue>) GetSubscriptionHandler() =>
            ("my channel", (_, _) => { }
        );
    }

    private IConnectionMultiplexerProvider _mockProvider;
    private IServiceCollection _mockServices;

    [SetUp]
    public void SetUp()
    {
        _mockProvider = Substitute.For<IConnectionMultiplexerProvider>();
        _mockServices = Substitute.For<IServiceCollection>();
    }

    [TestCase(RedisCapabilities.Keys)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.HashSet)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Publisher)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.Publisher)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.HashSet)]
    [TestCase(RedisCapabilities.HashSet | RedisCapabilities.Publisher)]
    [TestCase(RedisCapabilities.HashSet | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.HashSet | RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.Publisher)]
    [TestCase(RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.Subscriber)]
    public void AddRedis_WhenAnyCapabilityIsEnabledAndRedisConfigurationIsMissing_ShouldThrow(RedisCapabilities capabilities)
    {
        var config = new MemoryConfigurationSource
        {
            InitialData = []
        };

        Action action = () => _mockServices
            .AddRedis<StubSerdes>(new ConfigurationBuilder()
            .Add(config)
            .Build(), capabilities);

        action
            .Should()
            .ThrowExactly<KeyNotFoundException>()
            .WithMessage("Missing RedisConfiguration");
    }

    [TestCase(null, RedisCapabilities.Keys)]
    [TestCase(null, RedisCapabilities.Keys | RedisCapabilities.HashSet)]
    [TestCase(null, RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Publisher)]
    [TestCase(null, RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Subscriber)]
    [TestCase(null, RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(null, RedisCapabilities.Keys | RedisCapabilities.Publisher)]
    [TestCase(null, RedisCapabilities.Keys | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(null, RedisCapabilities.Keys | RedisCapabilities.Subscriber)]
    [TestCase(null, RedisCapabilities.HashSet)]
    [TestCase(null, RedisCapabilities.HashSet | RedisCapabilities.Publisher)]
    [TestCase(null, RedisCapabilities.HashSet | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(null, RedisCapabilities.HashSet | RedisCapabilities.Subscriber)]
    [TestCase(null, RedisCapabilities.Publisher)]
    [TestCase(null, RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(null, RedisCapabilities.Subscriber)]
    [TestCase("", RedisCapabilities.Keys)]
    [TestCase("", RedisCapabilities.Keys | RedisCapabilities.HashSet)]
    [TestCase("", RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Publisher)]
    [TestCase("", RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Subscriber)]
    [TestCase("", RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase("", RedisCapabilities.Keys | RedisCapabilities.Publisher)]
    [TestCase("", RedisCapabilities.Keys | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase("", RedisCapabilities.Keys | RedisCapabilities.Subscriber)]
    [TestCase("", RedisCapabilities.HashSet)]
    [TestCase("", RedisCapabilities.HashSet | RedisCapabilities.Publisher)]
    [TestCase("", RedisCapabilities.HashSet | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase("", RedisCapabilities.HashSet | RedisCapabilities.Subscriber)]
    [TestCase("", RedisCapabilities.Publisher)]
    [TestCase("", RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase("", RedisCapabilities.Subscriber)]
    [TestCase(" ", RedisCapabilities.Keys)]
    [TestCase(" ", RedisCapabilities.Keys | RedisCapabilities.HashSet)]
    [TestCase(" ", RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Publisher)]
    [TestCase(" ", RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Subscriber)]
    [TestCase(" ", RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(" ", RedisCapabilities.Keys | RedisCapabilities.Publisher)]
    [TestCase(" ", RedisCapabilities.Keys | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(" ", RedisCapabilities.Keys | RedisCapabilities.Subscriber)]
    [TestCase(" ", RedisCapabilities.HashSet)]
    [TestCase(" ", RedisCapabilities.HashSet | RedisCapabilities.Publisher)]
    [TestCase(" ", RedisCapabilities.HashSet | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(" ", RedisCapabilities.HashSet | RedisCapabilities.Subscriber)]
    [TestCase(" ", RedisCapabilities.Publisher)]
    [TestCase(" ", RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(" ", RedisCapabilities.Subscriber)]
    public void AddRedis_WhenAnyCapabilityIsEnabledAndRedisConfigurationIsNotValid_ShouldThrow(string configValue, RedisCapabilities capabilities)
    {
        var config = new MemoryConfigurationSource
        {
            InitialData = new Dictionary<string, string>()
        {
            {"RedisConfiguration:ConnectionString", configValue}
        }
        };

        Action action = () => _mockServices
            .AddRedis<StubSerdes>(new ConfigurationBuilder()
            .Add(config)
            .Build(), capabilities);

        action
            .Should()
            .ThrowExactly<KeyNotFoundException>()
            .WithMessage("RedisConfiguration: ConnectionString is invalid");
    }

    [TestCase(RedisCapabilities.Keys)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.HashSet)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Publisher)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Set)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.HashSet | RedisCapabilities.Publisher | RedisCapabilities.Subscriber | RedisCapabilities.Set)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.Publisher)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.Publisher | RedisCapabilities.Set)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.Subscriber | RedisCapabilities.Set)]
    [TestCase(RedisCapabilities.Keys | RedisCapabilities.Set)]
    [TestCase(RedisCapabilities.HashSet)]
    [TestCase(RedisCapabilities.HashSet | RedisCapabilities.Publisher)]
    [TestCase(RedisCapabilities.HashSet | RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.HashSet | RedisCapabilities.Publisher | RedisCapabilities.Set)]
    [TestCase(RedisCapabilities.HashSet | RedisCapabilities.Publisher | RedisCapabilities.Subscriber | RedisCapabilities.Set)]
    [TestCase(RedisCapabilities.HashSet | RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.HashSet | RedisCapabilities.Set)]
    [TestCase(RedisCapabilities.HashSet | RedisCapabilities.Subscriber | RedisCapabilities.Set)]
    [TestCase(RedisCapabilities.Publisher)]
    [TestCase(RedisCapabilities.Publisher | RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.Publisher | RedisCapabilities.Set)]
    [TestCase(RedisCapabilities.Publisher | RedisCapabilities.Subscriber | RedisCapabilities.Set)]
    [TestCase(RedisCapabilities.Subscriber)]
    [TestCase(RedisCapabilities.Subscriber | RedisCapabilities.Set)]
    [TestCase(RedisCapabilities.Set)]
    public void AddRedis_WhenAnyCapabilityIsEnabledAndConfigIsValidWithProperConnectionString_ShouldRegisterBasicComponents(RedisCapabilities capabilities)
    {
        var config = new MemoryConfigurationSource
        {
            InitialData = new Dictionary<string, string>
                    {
                        {"RedisConfiguration:ConnectionString", "redis-connection-string"},
                    },
        };

        _mockServices
            .AddRedis<StubSerdes>(new ConfigurationBuilder().Add(config).Build(), capabilities);

        _mockServices
            .Received(1)
            .Add(Arg.Is<ServiceDescriptor>(d =>
                    d.ServiceType == typeof(IConnectionMultiplexerProvider)
                        && d.ImplementationType == typeof(ConnectionMultiplexerProvider)
                    ));

        _mockServices
            .Received(1)
                .Add(Arg.Is<ServiceDescriptor>(d =>
                    d.ServiceType == typeof(ISourcesProvider)
                        && d.ImplementationType == typeof(RedisSourcesProvider)
                    ));

        _mockServices
            .Received(1)
            .Add(Arg.Is<ServiceDescriptor>(d =>
                    d.ServiceType == typeof(RedisConfiguration)
                        && ((RedisConfiguration)d.ImplementationInstance).ConnectionString == "redis-connection-string"
                    )
                );

        _mockServices
            .Received(1)
            .Add(Arg.Is<ServiceDescriptor>(d =>
                    d.ServiceType == typeof(IRedisSerDes)
                        && d.ImplementationType == typeof(StubSerdes)
                    )
                );
    }

    [TestCase(RedisCapabilities.HashSet, typeof(IRedisHashSetService), typeof(RedisHashSetService))]
    [TestCase(RedisCapabilities.Keys, typeof(IRedisKeyService), typeof(RedisKeyService))]
    [TestCase(RedisCapabilities.Set, typeof(IRedisSetService), typeof(RedisSetService))]
    public void AddRedis_WhenRedisHashSetIsEnabledAndConfigIsValid_ShouldRegisterComponents(
            RedisCapabilities capabilities,
            Type expectedKeyType,
            Type expectedImplementationType)
    {
        var config = new MemoryConfigurationSource
        {
            InitialData = new Dictionary<string, string>
            {
                {"RedisConfiguration:ConnectionString", "redis-connection-string"}
            }
        };

        _mockServices
            .AddRedis<StubSerdes>(new ConfigurationBuilder().Add(config).Build(), capabilities);

        _mockServices
            .Received(1)
            .Add(Arg.Is<ServiceDescriptor>(d =>
                    d.ServiceType == typeof(IConnectionMultiplexerProvider)
                        && d.ImplementationType == typeof(ConnectionMultiplexerProvider)
                    ));

        _mockServices
            .Received(1)
            .Add(Arg.Is<ServiceDescriptor>(d =>
                d.ServiceType == typeof(ISourcesProvider)
                    && d.ImplementationType == typeof(RedisSourcesProvider)
                ));

        _mockServices
            .Received(1)
                .Add(Arg.Is<ServiceDescriptor>(d =>
                    d.ServiceType == typeof(RedisConfiguration)
                        && ((RedisConfiguration)d.ImplementationInstance).ConnectionString == "redis-connection-string"
                    )
                );

        _mockServices
            .Received(1)
            .Add(Arg.Is<ServiceDescriptor>(d => d.ServiceType == expectedKeyType && d.ImplementationType == expectedImplementationType));

    }

    [TestCase(RedisCapabilities.HashSet, typeof(IRedisHashSetService), typeof(KeyTransformerRedisHashSetService))]
    [TestCase(RedisCapabilities.Keys, typeof(IRedisKeyService), typeof(KeyTransformerRedisKeyService))]
    [TestCase(RedisCapabilities.Set, typeof(IRedisSetService), typeof(KeyTransformerRedisSetService))]
    public void AddRedis_WhenRedisCapabilityIsEnabledAndKeyPrefixIsValidAndConfigIsValid_ShouldRegisterComponents(
            RedisCapabilities capabilities,
            Type expectedKeyType,
            Type expectedImplementationType)
    {
        var config = new MemoryConfigurationSource
        {
            InitialData = new Dictionary<string, string>
            {
                {"RedisConfiguration:ConnectionString", "redis-connection-string"},
                {"RedisKeyConfiguration:KeyPrefix", "whatever"}
            }
        };
        _mockServices = new ServiceCollection();

        _mockServices
            .AddRedis<StubSerdes>(new ConfigurationBuilder().Add(config).Build(), capabilities)
            .AddSingleton(_mockProvider);

        var provider = _mockServices.BuildServiceProvider();

        provider
            .GetRequiredService(expectedKeyType)
            .Should()
            .BeOfType(expectedImplementationType);
    }

    [TestCase(RedisCapabilities.Publisher, typeof(IRedisPublisherService), typeof(RedisPublisherService))]
    [TestCase(RedisCapabilities.Subscriber, typeof(IRedisSubscriber), typeof(TestRedisSubscriber))]
    public void AddRedis_WhenPublisherIsEnabledAndConfigIsValid_ShouldRegistercomponents(
            RedisCapabilities capabilities,
            Type expectedKeyType,
            Type expectedImplementationType)
    {
        var config = new MemoryConfigurationSource
        {
            InitialData = new Dictionary<string, string>
            {
                {"RedisConfiguration:ConnectionString", "whatever"}
            }
        };

        _mockServices
            .AddRedis<StubSerdes>(new ConfigurationBuilder().Add(config).Build(), capabilities, Assembly.GetExecutingAssembly());

        _mockServices
            .Received(1)
            .Add(Arg.Is<ServiceDescriptor>(d =>
                d.ServiceType == typeof(IConnectionMultiplexerProvider)
                    && d.ImplementationType == typeof(ConnectionMultiplexerProvider)
                ));

        _mockServices
            .Received(1)
            .Add(Arg.Is<ServiceDescriptor>(d =>
                d.ServiceType == typeof(ISourcesProvider)
                    && d.ImplementationType == typeof(RedisSourcesProvider)
                ));

        _mockServices
            .Received(1)
            .Add(Arg.Is<ServiceDescriptor>(d =>
                d.ServiceType == typeof(RedisConfiguration)
                    && ((RedisConfiguration)d.ImplementationInstance).ConnectionString == "whatever"
                )
            );

        _mockServices
            .Received(1)
            .Add(Arg.Is<ServiceDescriptor>(d => d.ServiceType == expectedKeyType && d.ImplementationType == expectedImplementationType));
    }

    [Test]
    public void AddSystemJsonRedisSerDes_ShouldAddSerDes() =>
        _mockServices
            .AddSystemJsonRedisSerDes()
            .Received(1)
            .Add(Arg.Is<ServiceDescriptor>(d =>
                d.ServiceType == typeof(IRedisSerDes)
                    && d.ImplementationType == typeof(SystemJsonRedisSerDes)
                ));

    [Test]
    public void AddRedisSerDes_WhenProvidingSystemJson_ShouldRegisterProperImplementation() =>
        _mockServices
            .AddRedisSerDes<SystemJsonRedisSerDes>()
            .Received(1)
            .Add(Arg.Is<ServiceDescriptor>(d =>
                d.ServiceType == typeof(IRedisSerDes)
                    && d.ImplementationType == typeof(SystemJsonRedisSerDes)
                ));

    [Test]
    public void AddRedisSerDes_WhenProvidingSpanJson_ShouldRegisterProperImplementation() =>
        _mockServices
            .AddRedisSerDes<SpanJsonRedisSerDes>()
            .Received(1)
            .Add(Arg.Is<ServiceDescriptor>(d =>
                d.ServiceType == typeof(IRedisSerDes)
                    && d.ImplementationType == typeof(SpanJsonRedisSerDes)
                ));
}