﻿namespace Func.Redis.Tests.LoggingRedisKeyService;

public partial class LoggingRedisKeyServiceTests
{

    [Test]
    public void Get_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data = new object();
        var output = Option<object>.Some(data);
        _mockService
            .Get<object>("some key")
            .Returns(output);

        var result = _sut.Get<object>("some key");

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: getting key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public void MultipleGet_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var data1 = new object();
        var data2 = new object();
        var output = new[] { Option<object>.Some(data1), Option<object>.Some(data2) };
        _mockService
            .Get<object>("key1", "key2")
            .Returns(output);

        var result = _sut.Get<object>("key1", "key2");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.Filter().Should().BeEquivalentTo([data1, data2]));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: getting keys \"key1, key2\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public void Get_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        var output = Option<object>.None();
        _mockService
            .Get<object>("some key")
            .Returns(Either<Error, Option<object>>.Right(output));

        var result = _sut.Get<object>("some key");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.IsNone.Should().BeTrue());

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: getting key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: key \"some key\" not found");
            e.LogLevel.Should().Be(LogLevel.Warning);
        });
    }

    [Test]
    public void MultipleGet_WhenServiceReturnsRightWithNone_ShouldReturnRightWithNone()
    {
        var output = new[] { Option<object>.None(), Option<object>.None() };
        _mockService
            .Get<object>("key1", "key2")
            .Returns(output);

        var result = _sut.Get<object>("key1", "key2");

        result.IsRight.Should().BeTrue();
        result.OnRight(r => r.Filter().Should().BeEmpty());

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: getting keys \"key1, key2\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public void Get_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .Get<object>("some key")
            .Returns(error);

        var result = _sut.Get<object>("some key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: getting key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void MultipleGet_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .Get<object>("key1", "key2")
            .Returns(error);

        var result = _sut.Get<object>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: getting keys \"key1, key2\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void Get_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .Get<object>("some key")
            .Returns(error);

        var result = _sut.Get<object>("some key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: getting key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public void MultipleGet_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .Get<object>("key1", "key2")
            .Returns(error);

        var result = _sut.Get<object>("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(r => r.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService: getting keys \"key1, key2\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisKeyService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}