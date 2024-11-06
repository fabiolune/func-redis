using AutoFixture;
using BenchmarkDotNet.Attributes;
using LanguageExt;
using LanguageExt.Common;
using Sport.Functional.Extensions;
using Sport.Redis;
using Sport.Redis.SerDes.Json;
using System.Diagnostics.CodeAnalysis;

namespace Benchmark;

[ExcludeFromCodeCoverage]
[MemoryDiagnoser]
public class SerDesBenchmark<T>
{
    private readonly RedisKeyService _systemJsonService;
    private readonly RedisKeyService _spanJsonService;

    private readonly T[] _data;

    public SerDesBenchmark()
    {
        _data = new Fixture()
            .Map<Fixture, IEnumerable<T>>(f => Enumerable.Range(0, 100).Select(_ => f.Create<T>()))
            .ToArray();

        var multiplexer = new ConnectionMultiplexerProvider(Configuration.Config);
        _spanJsonService = new(
            new RedisSourcesProvider(multiplexer),
            new SpanJsonRedisSerDes(),
            Configuration.KeyConfig);
        _systemJsonService = new(
            new RedisSourcesProvider(multiplexer),
            new SystemJsonRedisSerDes(),
            Configuration.KeyConfig);
    }

    private static EitherAsync<Error, Unit> Insert(IRedisService service, string prefix, T[] data) =>
        service
            .SetAsync(prefix, data)
            .MapLeft(e => e.Tee(_ => Console.WriteLine($"SET {prefix}: exception {e.Message}")));

    private static Task<Unit> InsertAndRead(IRedisService service, string prefix, T[] data) =>
        Insert(service, prefix, data)
            .Bind(_ => service.GetAsync<T[]>(prefix))
            .Match(o => o.IfNone(() => Console.WriteLine($"{prefix}: no data")), e => Console.WriteLine($"GET {prefix}: exception {e.Message}"));

    [Benchmark]
    public EitherAsync<Error, Unit> SystemJsonInsert() => Insert(_systemJsonService, "system", _data);

    [Benchmark]
    public EitherAsync<Error, Unit> SpanJsonInsert() => Insert(_spanJsonService, "span", _data);

    [Benchmark]
    public Task<Unit> SpanJsonInsertAndRead() => InsertAndRead(_spanJsonService, "span", _data);

    [Benchmark]
    public Task<Unit> SystemJsonInsertAndRead() => InsertAndRead(_systemJsonService, "system", _data);
}