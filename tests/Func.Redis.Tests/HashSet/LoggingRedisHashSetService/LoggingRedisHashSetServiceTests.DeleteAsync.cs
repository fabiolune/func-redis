﻿namespace Func.Redis.Tests.LoggingRedisHashSetService;

public partial class LoggingRedisHashSetServiceTest
{

    [Test]
    public async Task DeleteAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        _mockService
            .DeleteAsync("some key", "some field")
            .Returns(Unit.Default);

        var result = await _sut.DeleteAsync("some key", "some field");

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: async deleting field \"some field\" from key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task DeleteAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var error = Error.New("some message");
        _mockService
            .DeleteAsync("some key", "some field")
            .Returns(error);

        var result = await _sut.DeleteAsync("some key", "some field");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: async deleting field \"some field\" from key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task DeleteAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .DeleteAsync("some key", "some field")
            .Returns(error);

        var result = await _sut.DeleteAsync("some key", "some field");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: async deleting field \"some field\" from key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultiDeleteAsync_WhenServiceReturnsRightWithSome_ShouldReturnRightWithSome()
    {
        var fields = new[] { "some field 1", "some field 2" };
        _mockService
            .DeleteAsync("some key", fields)
            .Returns(Unit.Default);

        var result = await _sut.DeleteAsync("some key", fields);

        result.IsRight.Should().BeTrue();

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(1);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: async deleting fields \"some field 1, some field 2\" from key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
    }

    [Test]
    public async Task MultiDeleteAsync_WhenServiceReturnsLeft_ShouldReturnLeft()
    {
        var fields = new[] { "some field 1", "some field 2" };
        var error = Error.New("some message");
        _mockService
            .DeleteAsync("some key", fields)
            .Returns(error);

        var result = await _sut.DeleteAsync("some key", fields);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: async deleting fields \"some field 1, some field 2\" from key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }

    [Test]
    public async Task MultiDeleteAsync_WhenServiceReturnsLeftWithException_ShouldReturnLeft()
    {
        var fields = new[] { "some field 1", "some field 2" };
        var exception = new Exception("some message");
        var error = Error.New(exception);
        _mockService
            .DeleteAsync("some key", fields)
            .Returns(error);

        var result = await _sut.DeleteAsync("some key", fields);

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().Be(error));

        var entries = _loggerFactory.Sink.LogEntries.ToArray();
        entries.Should().HaveCount(2);
        entries[0].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService: async deleting fields \"some field 1, some field 2\" from key \"some key\"");
            e.LogLevel.Should().Be(LogLevel.Information);
        });
        entries[1].Should().BeOfType<LogEntry>().Which.Tee(e =>
        {
            e.Message.Should().Be("IRedisHashSetService raised an error with some message");
            e.LogLevel.Should().Be(LogLevel.Error);
        });
    }
}