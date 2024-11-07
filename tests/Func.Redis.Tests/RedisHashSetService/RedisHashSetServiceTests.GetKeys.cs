namespace Func.Redis.Tests.RedisHashSetService;

public partial class RedisHashSetServiceTests
{

    [Test]
    public void GetKeys_WhenDatabaseIsNull_ShouldReturnError()
    {
        _mockProvider.GetDatabase().Returns(null as IDatabase);
        _sut = new Redis.RedisHashSetService(_mockProvider, _mockSerDes);

        _mockProvider
            .GetDatabase()
            .Returns(null as IDatabase);

        var result = _sut.GetFieldKeys("key");

        result.IsLeft.Should().BeTrue();
        result.OnLeft(err => err.Should().Be(Error.New(new NullReferenceException())));
    }

    [Test]
    public void GetKeys_WhenDatabaseThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some message");

        _mockDb
            .HashKeys("key", Arg.Any<CommandFlags>())
            .Returns(_ => throw exception);

        var result = _sut.GetFieldKeys("key");

        result.IsLeft.Should().BeTrue();
        result
            .OnLeft(e => e.Should().BeEquivalentTo(Error.New(exception)));
        _mockDb
            .Received(1)
            .HashKeys("key", Arg.Any<CommandFlags>());
    }

    [Test]
    public void GetKeys_WhenDatabaseReturnsItemWithNoValue_ShouldReturnRightWithNone()
    {
        _mockDb
            .HashKeys("key", Arg.Any<CommandFlags>())
            .Returns([]);

        var result = _sut.GetFieldKeys("key");

        result.IsRight.Should().BeTrue();
        result
            .OnRight(e => e.IsNone.Should().BeTrue());
        _mockDb
            .Received(1)
            .HashKeys("key", Arg.Any<CommandFlags>());
    }

    [TestCase("some-id", "some-id2")]
    public void GetKeys_WhenDatabaseReturnsMoreString_ShouldReturnRightWithSome(string serializedData, string serializedData2)
    {
        var expectedData = new string[] { "some-id", "some-id2" };
        _mockDb
            .HashKeys("key", Arg.Any<CommandFlags>())
            .Returns([serializedData, serializedData2]);

        var result = _sut.GetFieldKeys("key");

        result.IsRight.Should().BeTrue(); 
        result
            .OnRight(e => e.OnSome(d => d.Should().BeEquivalentTo(expectedData)));
    }
}