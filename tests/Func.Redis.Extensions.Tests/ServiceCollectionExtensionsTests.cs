using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Sport.Redis.Models;
using Sport.Redis.SerDes;
using Sport.Redis.SerDes.Json;
using StackExchange.Redis;
using System.Reflection;

namespace Sport.Redis.Extensions.Unit.Tests;

public class ServiceCollectionExtensionsTests
{

    private IServiceCollection _mockServices;

    [SetUp]
    public void SetUp() => _mockServices = Substitute.For<IServiceCollection>();

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
            InitialData = new Dictionary<string, string>()
        };

        Action action = () => _mockServices
            .AddRedis(new ConfigurationBuilder()
            .Add(config)
            .Build(), capabilities);

        action
            .Should()
            .ThrowExactly<Exception>()
            .WithMessage("RedisConfiguration section not found in Configuration");
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
            .AddRedis(new ConfigurationBuilder()
            .Add(config)
            .Build(), capabilities);

        action
            .Should()
            .ThrowExactly<Exception>()
            .WithMessage("RedisConfiguration: ConnectionString is invalid");
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
    public void AddRedis_WhenAnyCapabilityIsEnabledAndConfigIsValid_ShouldRegisterComponents(RedisCapabilities capabilities)
    {
        var config = new MemoryConfigurationSource
        {
            InitialData = new Dictionary<string, string>
                    {
                        {"RedisConfiguration:ConnectionString", "whatever"},
                        {"RedisServiceConfiguration:KeyPrefix", "whatever"}
                    },
        };

        _mockServices
            .AddRedis(new ConfigurationBuilder().Add(config).Build(), capabilities);

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
    }

    [TestCase(RedisCapabilities.HashSet, typeof(IRedisHashSetService), typeof(RedisHashSetService))]
    [TestCase(RedisCapabilities.Keys, typeof(IRedisService), typeof(RedisKeyService))]
    public void AddRedis_WhenRedisHashSetIsEnabledAndConfigIsValid_ShouldRegisterComponents(
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
            .AddRedis(new ConfigurationBuilder().Add(config).Build(), capabilities);

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
            .AddRedis(new ConfigurationBuilder().Add(config).Build(), capabilities, Assembly.GetExecutingAssembly());

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

    internal class TestRedisSubscriber : IRedisSubscriber
    {
        public (string, Action<RedisChannel, RedisValue>) GetSubscriptionHandler() =>
            ("my channel", (_, _) => { }
        );
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