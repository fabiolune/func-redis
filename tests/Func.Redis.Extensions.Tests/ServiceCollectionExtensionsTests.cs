using Func.Redis.HashSet;
using Func.Redis.Key;
using Func.Redis.List;
using Func.Redis.Publisher;
using Func.Redis.SerDes;
using Func.Redis.SerDes.Json;
using Func.Redis.Set;
using Func.Redis.SortedSet;
using Func.Redis.Subscriber;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TinyFp.Extensions;

namespace Func.Redis.Extensions.Unit.Tests;

internal class ServiceCollectionExtensionsTests
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

    internal static readonly RedisCapabilities[] AllCapabilities =
        Enumerable
            .Range(1, Enum.GetValues<RedisCapabilities>().Select(c => (int)c).Max() << 1 - 1)
            .Select(i => (RedisCapabilities)i)
            .ToArray();

    internal static readonly object[][] InvalidConfigAndAllCapabilities =
        new[] { "", null }
            .Map(inv => AllCapabilities.SelectMany(c => inv.Select(i => new object[] { i, c })))
            .ToArray();

    [SetUp]
    public void SetUp()
    {
        _mockProvider = Substitute.For<IConnectionMultiplexerProvider>();
        _mockServices = Substitute.For<IServiceCollection>();
    }

    [TestCaseSource(nameof(AllCapabilities))]
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

    [TestCaseSource(nameof(InvalidConfigAndAllCapabilities))]
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

    [TestCaseSource(nameof(AllCapabilities))]
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
    [TestCase(RedisCapabilities.Key, typeof(IRedisKeyService), typeof(RedisKeyService))]
    [TestCase(RedisCapabilities.Set, typeof(IRedisSetService), typeof(RedisSetService))]
    [TestCase(RedisCapabilities.SortedSet, typeof(IRedisSortedSetService), typeof(RedisSortedSetService))]
    [TestCase(RedisCapabilities.List, typeof(IRedisListService), typeof(RedisListService))]
    [TestCase(RedisCapabilities.Generic, typeof(IRedisService), typeof(RailwayRedisService))]
    public void AddRedis_WhenCapabilityIsEnabledAndConfigIsValid_ShouldRegisterComponents(
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
    [TestCase(RedisCapabilities.Key, typeof(IRedisKeyService), typeof(KeyTransformerRedisKeyService))]
    [TestCase(RedisCapabilities.Set, typeof(IRedisSetService), typeof(KeyTransformerRedisSetService))]
    [TestCase(RedisCapabilities.SortedSet, typeof(IRedisSortedSetService), typeof(KeyTransformerRedisSortedSetService))]
    [TestCase(RedisCapabilities.List, typeof(IRedisListService), typeof(KeyTransformerRedisListService))]
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

    [TestCase(RedisCapabilities.HashSet, typeof(IRedisHashSetService), typeof(LoggingRedisHashSetService))]
    [TestCase(RedisCapabilities.Key, typeof(IRedisKeyService), typeof(LoggingRedisKeyService))]
    [TestCase(RedisCapabilities.Set, typeof(IRedisSetService), typeof(LoggingRedisSetService))]
    [TestCase(RedisCapabilities.SortedSet, typeof(IRedisSortedSetService), typeof(LoggingRedisSortedSetService))]
    [TestCase(RedisCapabilities.List, typeof(IRedisListService), typeof(LoggingRedisListService))]
    [TestCase(RedisCapabilities.Publish, typeof(IRedisPublisherService), typeof(LoggingRedisPublisherService))]
    public void AddLoggingRedis_WhenRedisCapabilityIsEnabledAndKeyPrefixIsValidAndConfigIsValid_ShouldRegisterComponents(
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
        _mockServices = new ServiceCollection();

        _mockServices
            .AddRedis<StubSerdes>(new ConfigurationBuilder().Add(config).Build(), capabilities, true)
            .AddLogging()
            .AddSingleton(_mockProvider);

        var provider = _mockServices.BuildServiceProvider();

        provider
            .GetRequiredService(expectedKeyType)
            .Should()
            .BeOfType(expectedImplementationType);
    }

    [TestCase(RedisCapabilities.Publish, typeof(IRedisPublisherService), typeof(RedisPublisherService))]
    [TestCase(RedisCapabilities.Subscribe, typeof(IRedisSubscriber), typeof(TestRedisSubscriber))]
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
            .AddRedis<StubSerdes>(new ConfigurationBuilder().Add(config).Build(), capabilities, false, Assembly.GetExecutingAssembly());

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