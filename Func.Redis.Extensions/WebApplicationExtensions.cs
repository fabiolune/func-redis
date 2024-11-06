using Microsoft.Extensions.DependencyInjection;
using Sport.Functional.Extensions;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;
using WebApp = Microsoft.AspNetCore.Builder.WebApplication;

namespace Sport.Redis.Extensions;

[ExcludeFromCodeCoverage(Justification = "static extension methods")]
public static class WebApplicationExtensions
{
    public static WebApp UseRedisSubcribers(this WebApp app) =>
        app
           .Tee(a => a
                .Services
                .Map(s => (s.GetRequiredService<IEnumerable<IRedisSubscriber>>(), s.GetRequiredService<IConnectionMultiplexerProvider>()))
                .Map(t => (Handlers: t.Item1.Select(s => s.GetSubscriptionHandler()), Mux: t.Item2.GetMultiplexer()))
                .Do(t => t
                    .Handlers
                    .ForEach(tt => t.Mux.GetSubscriber().Subscribe(RedisChannel.Literal(tt.Item1), tt.Item2))));
}