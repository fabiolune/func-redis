using StackExchange.Redis;
using TinyFp.Extensions;

namespace Func.Redis.Utils;

internal static class FunctionUtilities
{
    internal static Either<Error, Unit> Wrap(Func<bool> func, Error error) =>
        Try(() => func())
            .ToEither()
            .MapLeft(e => Error.New(e.Message))
            .Bind(res => res.ToEither(_ => Unit.Default, b => !b, error));

    internal static Either<Error, TOut> Wrap<TIn, TOut>(Func<TIn> func, Func<TIn, TOut> map) =>
        Try(() => func())
            .Map(map)
            .ToEither()
            .MapLeft(e => Error.New(e.Message));

    internal static Either<Error, T> Wrap<T>(Func<T> func) =>
        Try(() => func())
            .ToEither()
            .MapLeft(e => Error.New(e.Message));

    internal static Either<Error, TOut> WrapUnsafe<TIn, TOut>(Func<TIn> func, Func<TIn, TOut> map) =>
        Try(() => func().Map(map))
            .ToEither()
            .MapLeft(e => Error.New(e.Message));

    internal static Task<Either<Error, Unit>> WrapAsync(Func<Task<bool>> func, Error error) =>
        TryAsync(() => func())
            .ToEither()
            .MapLeftAsync(e => Error.New(e.Message))
            .BindAsync(res => res.ToEither(_ => Unit.Default, b => !b, error));

    internal static Task<Either<Error, TOut>> WrapAsync<TIn, TOut>(Func<Task<TIn>> func, Func<TIn, TOut> map) =>
        TryAsync(() => func())
            .ToEither()
            .MapLeftAsync(e => Error.New(e.Message))
            .MapAsync(map);

    internal static Task<Either<Error, T>> WrapAsync<T>(Func<Task<T>> func) =>
        TryAsync(() => func())
            .ToEither()
            .MapLeftAsync(e => Error.New(e.Message));

    internal static Task<Either<Error, TOut>> WrapUnsafeAsync<TIn, TOut>(Func<Task<TIn>> func, Func<TIn, TOut> map) =>
        TryAsync(() => func())
            .ToEither()
            .MapLeftAsync(e => Error.New(e.Message))
            .BindAsync(r => Try(() => map(r)).ToEither().MapLeft(e => Error.New(e.Message)));
}
