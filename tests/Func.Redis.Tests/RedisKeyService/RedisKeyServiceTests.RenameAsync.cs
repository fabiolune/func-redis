namespace Func.Redis.Tests.RedisKeyService;

public partial class RedisKeyServiceTests
{
    [Test]
    public async Task RenameKeyAsync_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .KeyRenameAsync("key1", "key2", When.Always, CommandFlags.None)
            .Returns<bool>(_ => throw exception);

        var result = await _sut.RenameKeyAsync("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }

    [Test]
    public async Task RenameKeyAsync_WhenDatabaseReturnsFalse_ShouldReturnError()
    {
        _mockDb
            .KeyRenameAsync("key1", "key2", When.Always, CommandFlags.None)
            .Returns(false);

        var result = await _sut.RenameKeyAsync("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Error renaming key")));
        await _mockDb
            .Received(1)
            .KeyRenameAsync("key1", "key2", When.Always, CommandFlags.None);
    }

    [Test]
    public async Task RenameKeyAsync_WhenDatabaseReturnsTrue_ShouldReturnError()
    {
        _mockDb
            .KeyRenameAsync("key1", "key2", When.Always, CommandFlags.None)
            .Returns(true);

        var result = await _sut.RenameKeyAsync("key1", "key2");

        result.IsRight.Should().BeTrue();
        await _mockDb
            .Received(1)
            .KeyRenameAsync("key1", "key2", When.Always, CommandFlags.None);
    }
}