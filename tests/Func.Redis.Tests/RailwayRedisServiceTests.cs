namespace Func.Redis.Tests;
internal class RailwayRedisServiceTests
{
    private RailwayRedisService _sut;
    private IDatabase _mockDb;

    [SetUp]
    public void SetUp()
    {
        _mockDb = Substitute.For<IDatabase>();
        var provider = Substitute.For<ISourcesProvider>();
        provider.GetDatabase().Returns(_mockDb);
        _sut = new RailwayRedisService(provider);
    }

    [Test]
    public void Execute_WhenExecuted_ShouldReturnRight()
    {
        static RedisValue exec(IDatabase db) => "test";

        var result = _sut.Execute(exec);

        result.IsRight.ShouldBeTrue();
    }

    [Test]
    public void Execute_WhenFuncThrows_ShouldReturnLeft()
    {
        static RedisValue exec(IDatabase db) => throw new Exception("test");

        var result = _sut.Execute(exec);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(Error.New("test")));
    }

    [Test]
    public void ExecuteWithMap_WhenExecuted_ShouldReturnRight()
    {
        static TestData exec(IDatabase db) => new(27);
        static int map(TestData value) => value.Id;

        var result = _sut.Execute(exec, map);

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.ShouldBe(27));
    }

    [Test]
    public void ExecuteWithMap_WhenFuncThrows_ShouldReturnLeft()
    {
        static TestData exec(IDatabase db) => throw new Exception("test");
        static int map(TestData value) => value.Id;

        var result = _sut.Execute(exec, map);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(Error.New("test")));
    }

    [Test]
    public void ExecuteWithMap_WhenMapThrows_ShouldReturnLeft()
    {
        static TestData exec(IDatabase db) => new(27);
        static int map(TestData value) => throw new Exception("test");

        var result = _sut.Execute(exec, map);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(Error.New("test")));
    }

    [Test]
    public async Task ExecuteAsync_WhenExecuted_ShouldReturnRight()
    {
        static Task<RedisValue> exec(IDatabase db) => Task.FromResult((RedisValue)"test");

        var result = await _sut.ExecuteAsync(exec);

        result.IsRight.ShouldBeTrue();
    }

    [Test]
    public async Task ExecuteAsync_WhenFuncThrows_ShouldReturnLeft()
    {
        static Task<RedisValue> exec(IDatabase db) => throw new Exception("test");

        var result = await _sut.ExecuteAsync(exec);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(Error.New("test")));
    }

    [Test]
    public async Task ExecuteAsync_WhenFuncThrowsAsync_ShouldReturnLeft()
    {
        static Task<RedisValue> exec(IDatabase db) => Task.FromException<RedisValue>(new Exception("test"));

        var result = await _sut.ExecuteAsync(exec);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(Error.New("test")));
    }

    [Test]
    public async Task ExecuteAsyncWithMap_WhenExecuted_ShouldReturnRight()
    {
        static Task<TestData> exec(IDatabase db) => Task.FromResult(new TestData(27));
        static int map(TestData value) => value.Id;

        var result = await _sut.ExecuteAsync(exec, map);

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.ShouldBe(27));
    }

    [Test]
    public async Task ExecuteAsyncWithMap_WhenFuncThrows_ShouldReturnLeft()
    {
        static Task<TestData> exec(IDatabase db) => throw new Exception("test");
        static int map(TestData value) => value.Id;

        var result = await _sut.ExecuteAsync(exec, map);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(Error.New("test")));
    }

    [Test]
    public async Task ExecuteAsyncWithMap_WhenFuncThrowsAsync_ShouldReturnLeft()
    {
        static Task<TestData> exec(IDatabase db) => Task.FromException<TestData>(new Exception("test"));
        static int map(TestData value) => value.Id;

        var result = await _sut.ExecuteAsync(exec, map);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(Error.New("test")));
    }

    [Test]
    public async Task ExecuteAsyncWithMap_WhenMapThrows_ShouldThrow()
    {
        static Task<TestData> exec(IDatabase db) => Task.FromResult(new TestData(27));
        static int map(TestData value) => throw new Exception("test");

        Func<Task> act = () => _sut.ExecuteAsync(exec, map);

        var ex = await act.ShouldThrowAsync<Exception>();
        ex.Message.ShouldBe("test");
    }

    [Test]
    public async Task ExecuteUnsafeAsyncWithMap_WhenExecuted_ShouldReturnRight()
    {
        static Task<TestData> exec(IDatabase db) => Task.FromResult(new TestData(27));
        static int map(TestData value) => value.Id;

        var result = await _sut.ExecuteUnsafeAsync(exec, map);

        result.IsRight.ShouldBeTrue();
        result.OnRight(r => r.ShouldBe(27));
    }

    [Test]
    public async Task ExecuteUnsafeAsyncWithMap_WhenFuncThrows_ShouldReturnLeft()
    {
        static Task<TestData> exec(IDatabase db) => throw new Exception("test");
        static int map(TestData value) => value.Id;

        var result = await _sut.ExecuteUnsafeAsync(exec, map);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(Error.New("test")));
    }

    [Test]
    public async Task ExecuteUnsafeAsyncWithMap_WhenFuncThrowsAsync_ShouldReturnLeft()
    {
        static Task<TestData> exec(IDatabase db) => Task.FromException<TestData>(new Exception("test"));
        static int map(TestData value) => value.Id;

        var result = await _sut.ExecuteUnsafeAsync(exec, map);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(Error.New("test")));
    }

    [Test]
    public async Task ExecuteUnsafeAsyncWithMap_WhenMapThrows_ShouldReturnLeft()
    {
        static Task<TestData> exec(IDatabase db) => Task.FromResult(new TestData(27));
        static int map(TestData value) => throw new Exception("test");

        var result = await _sut.ExecuteUnsafeAsync(exec, map);

        result.IsLeft.ShouldBeTrue();
        result.OnLeft(e => e.ShouldBe(Error.New("test")));
    }
}
