namespace Func.Redis.Tests.RedisKeyService;

public partial class RedisKeyServiceTests
{
    [Test]
    public void RenameKey_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .KeyRename("key1", "key2", When.Always, CommandFlags.None)
            .Returns(_ => throw exception);

        var result = _sut.RenameKey("key1", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
    }

    [Test]
    public void RenameKey_WhenDatabaseReturnsFalse_ShouldReturnError()
    {
        _mockDb
            .KeyRename("key1", "key2", When.Always, CommandFlags.None)
            .Returns(false);

        var result = _sut.RenameKey("key", "key2");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(e => e.Should().BeEquivalentTo(Error.New("Error renaming key")));
    }

    [Test]
    public void RenameKey_WhenDatabaseReturnsTrue_ShouldReturnError()
    {
        _mockDb
            .KeyRename("key1", "key2", When.Always, CommandFlags.None)
            .Returns(true);

        var result = _sut.RenameKey("key1", "key2");

        result.IsRight.Should().BeTrue();
    }
}