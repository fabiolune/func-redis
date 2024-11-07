using Func.Redis.SerDes.Json;
using System.Text.Json;

namespace Func.Redis.Tests.SerDes.Json;

public class SystemJsonRedisSerDesTests
{
    private SystemJsonRedisSerDes _sut;

    public class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [SetUp]
    public void SetUp() => _sut = new SystemJsonRedisSerDes();

    public static readonly RedisValue[] InvalidRedisValues =
    [
        RedisValue.Null,
        RedisValue.EmptyString,
        RedisValue.Null
    ];

    [TestCase(@"{""Id"": 12}")]
    [TestCase(@"{""id"": 12}")]
    public void Deserialize_WhenInputIsValidJson_ShouldReturnSome(string serialized)
    {
        var result = _sut.Deserialize<TestData>(serialized);

        result.IsSome.Should().BeTrue();
        result
            .OnSome(data =>
                data
                    .Should()
                    .BeEquivalentTo(new TestData { Id = 12 }));
    }

    [TestCaseSource(nameof(InvalidRedisValues))]
    public void Deserialize_WhenInputIsNullOrEmptyString_ShouldReturnNone(RedisValue value) =>
        _sut
            .Deserialize<TestData>(value)
            .IsNone
            .Should()
            .BeTrue();

    [Test]
    public void Deserialize_WhenInputIsNullJson_ShouldReturnNone() =>
        _sut
            .Deserialize<TestData>("null")
            .IsNone
            .Should()
            .BeTrue();

    [Test]
    public void Deserialize_WhenInputIsInvalidJson_ShouldThrowJsonException()
    {
        var act = () => _sut.Deserialize<TestData>("{ wrong json");

        act.Should().ThrowExactly<JsonException>();
    }

    [TestCase(@"{""Id"": 12}")]
    [TestCase(@"{""id"": 12}")]
    public void DeserializeWithType_WhenInputIsValidJson_ShouldReturnSome(string serialized)
    {
        var result = _sut.Deserialize(serialized, typeof(TestData));

        result.IsSome.Should().BeTrue();
        result
            .OnSome(data =>
                data
                    .Should()
                    .BeEquivalentTo(new TestData { Id = 12 }));
    }

    [TestCaseSource(nameof(InvalidRedisValues))]
    public void DeserializeWithType_WhenInputIsNullOrEmptyString_ShouldReturnNone(RedisValue value) =>
        _sut
            .Deserialize(value, typeof(TestData))
            .IsNone
            .Should()
            .BeTrue();

    [Test]
    public void DeserializeWithType_WhenInputIsNullJson_ShouldReturnNone() =>
        _sut
            .Deserialize("null", typeof(TestData))
            .IsNone
            .Should()
            .BeTrue();

    [Test]
    public void DeserializeWithType_WhenInputIsInvalidJson_ShouldThrowJsonException()
    {
        var act = () => _sut.Deserialize("{ wrong json", typeof(TestData));

        act.Should().ThrowExactly<JsonException>();
    }

    [Test]
    public void Deserialize_WhenInputHasZeroLength_ShouldReturnNone() =>
        _sut
            .Deserialize<TestData>(Array.Empty<RedisValue>())
            .IsNone
            .Should()
            .BeTrue();

    public static readonly RedisValue[][] InvalidValues =
    [
        [
            RedisValue.Null,
            "something"
        ],
        [
            "something",
            RedisValue.Null
        ],
        [
            RedisValue.EmptyString,
            "something"
        ],
        [
            "something",
            RedisValue.EmptyString
        ],
        [
            "null",
            "something"
        ],
        [
            "something",
            "null"
        ]
    ];

    [TestCaseSource(nameof(InvalidValues))]
    public void Deserialize_WhenInputHasSomeNullOrEmptyValues_ShouldReturnNone(RedisValue[] values) =>
        _sut
            .Deserialize<TestData>(values)
            .IsNone
            .Should()
            .BeTrue();

    [Test]
    public void Deserialize_WhenValuesAreValidJson_ShouldReturnSome()
    {
        var result = _sut.Deserialize<TestData>([@"{""Id"": 12}", @"{""id"": 27}"]);

        result.IsSome.Should().BeTrue();
        result
            .OnSome(v => v.Should().BeEquivalentTo(new[]
            {
            new TestData { Id = 12 },
            new TestData { Id = 27 }
            }));
    }

    public static readonly RedisValue[][] InvalidJsonValues =
    [
        [
            "{}",
            "{ wrong json"
        ],
        [
            "{ wrong json",
            @"{""Id"": 42}"
        ],
        [
            "{ wrong json",
            @"{""id"": 42}"
        ]
    ];

    [TestCaseSource(nameof(InvalidJsonValues))]
    public void Deserialize_WhenInputContainInvalidJson_ShouldThrowJsonException(RedisValue[] values)
    {
        var act = () => _sut.Deserialize<TestData>(values);

        act.Should().ThrowExactly<JsonException>();
    }

    [Test]
    public void Deserialize_WhenHashEntriesAreEmpty_ShouldReturnNone() =>
        _sut
            .Deserialize<TestData>(Array.Empty<HashEntry>())
            .IsNone
            .Should()
            .BeTrue();

    public static readonly HashEntry[][] InvalidEntries =
    [
        [
            new HashEntry("valid", RedisValue.Null),
            new HashEntry("valid", "{}")
        ],
        [
            new HashEntry("valid", "{}"),
            new HashEntry("valid", RedisValue.Null)
        ],
        [
            new HashEntry("valid", RedisValue.EmptyString),
            new HashEntry("valid", "{}")
        ],
        [
            new HashEntry("valid", "{}"),
            new HashEntry("valid", RedisValue.EmptyString)
        ],
        [
            new HashEntry("valid", "null"),
            new HashEntry("valid", "{}")
        ],
        [
            new HashEntry("valid", "{}"),
            new HashEntry("valid", "null")
        ]
    ];

    [TestCaseSource(nameof(InvalidEntries))]
    public void DeserializeEntries_WhenEntriesContainInvalidData_ShouldReturnNone(HashEntry[] entries) =>
        _sut
            .Deserialize<TestData>(entries)
            .IsNone
            .Should()
            .BeTrue();

    [Test]
    public void DeserializeEntries_WhenEntriesContainValidJson_ShouldReturnSome()
    {
        var result = _sut.Deserialize<TestData>([new HashEntry("key1", @"{""Id"": 12}"), new HashEntry("key2", @"{""id"": 27}")]);

        result.IsSome.Should().BeTrue();
        result
            .OnSome(v => v.Should().BeEquivalentTo(new (string, TestData)[]
            {
                ("key1", new TestData { Id = 12 }),
                ("key2", new TestData { Id = 27 })
            }));
    }

    public static readonly HashEntry[][] InvalidJsonEntries =
    [
        [
            new HashEntry("key1", "{}"),
            new HashEntry("key2", "{ wrong json")
        ],
        [
            new HashEntry("key1", "{ wrong json"),
            new HashEntry("key2", @"{""Id"": 42}")
        ],
        [
            new HashEntry("key1", "{ wrong json"),
            new HashEntry("key2", @"{""id"": 42}")
        ]
    ];

    [TestCaseSource(nameof(InvalidJsonEntries))]
    public void Deserialize_WhenEntriesContainInvalidJson_ShouldThrowJsonException(HashEntry[] entries)
    {
        var act = () => _sut.Deserialize<TestData>(entries);

        act.Should().ThrowExactly<JsonException>();
    }

    public static readonly object[][] SerializationCombinations =
    [
        [new TestData { Id = 17 }, @"{""Id"":17}"],
        [new TestData { Id = 17, Name = "some name" }, @"{""Id"":17,""Name"":""some name""}"],
        [null, "null"],
        ["message", @"""message"""]
    ];

    [TestCaseSource(nameof(SerializationCombinations))]
    public void Serialize_WnenInputIsValid_ShouldReturnJsonRepresentation(object item, string serialization) =>
        _sut
            .Serialize(item)
            .Should()
            .Be(serialization);
}