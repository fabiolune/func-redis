using Microsoft.Extensions.Logging;
using TinyFp.Extensions;

namespace Func.Redis.Extensions;
public static class EitherLoggingExtensions
{
    internal static Either<Error, T> TeeLog<T>(this Either<Error, T> self, ILogger logger, string component) =>
        self.MapLeft(err => err.Tee(e => logger.LogError("{Component} raised an error with {Message}", component, e.Message)));

    internal static Task<Either<Error, T>> TeeLog<T>(this Task<Either<Error, T>> self, ILogger logger, string component) =>
        self.MapLeftAsync(err => err.Tee(e => logger.LogError("{Component} raised an error with {Message}", component, e.Message)));
}
