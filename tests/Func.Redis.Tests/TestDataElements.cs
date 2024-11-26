namespace Func.Redis.Tests;
internal static class TestDataElements
{
    internal record TestData
    {
        public TestData() { }
        public TestData(int id) => Id = id;
        public int Id { get; init; }
        public string Name { get; set; }
    }

    private static readonly Either<Error, Unit> RightErrorUnit = Either<Error, Unit>.Right(Unit.Default);
    private static readonly Either<Error, Unit> LeftErrorUnit = Either<Error, Unit>.Left(Error.New("error"));

    public static readonly Either<Error, Unit>[] ErrorUnitTestData =
    [
        RightErrorUnit,
        LeftErrorUnit
    ];

    private static readonly Either<Error, Option<string>> SomeErrorOptionString = Either<Error, Option<string>>.Right(Option<string>.Some("success"));
    private static readonly Either<Error, Option<string>> NoneErrorOptionString = Either<Error, Option<string>>.Right(Option<string>.None());
    private static readonly Either<Error, Option<string>> LeftErrorOptionString = Either<Error, Option<string>>.Left(Error.New("error"));

    public static readonly Either<Error, Option<string>>[] ErrorStringTestData =
       [
            SomeErrorOptionString,
            NoneErrorOptionString,
            LeftErrorOptionString
       ];

    private static readonly Either<Error, Option<string>[]> SomeErrorOptionStrings = Either<Error, Option<string>[]>.Right([Option<string>.Some("success1"), Option<string>.None()]);
    private static readonly Either<Error, Option<string>[]> LeftErrorOptionStrings = Either<Error, Option<string>[]>.Left(Error.New("error"));

    public static readonly Either<Error, Option<string>[]>[] ErrorOptionStringsTestData =
    [
        SomeErrorOptionStrings,
        LeftErrorOptionStrings
    ];

    private static readonly Either<Error, Option<object>[]> SomeErrorOptionObjects = Either<Error, Option<object>[]>.Right([Option<object>.Some(new TestData(1)), Option<object>.None()]);
    private static readonly Either<Error, Option<object>[]> LeftErrorOptionObjects = Either<Error, Option<object>[]>.Left(Error.New("error"));

    public static readonly Either<Error, Option<object>[]>[] ErrorObjectsTestData =
        [
            SomeErrorOptionObjects,
            LeftErrorOptionObjects
        ];

    private static readonly Either<Error, Option<(string, TestData)[]>> SomeErrorTuples = Either<Error, Option<(string, TestData)[]>>.Right(new[] { ("first", new TestData(1)), ("second", new TestData(2)) }.ToOption());
    private static readonly Either<Error, Option<(string, TestData)[]>> NoneErrorTuples = Either<Error, Option<(string, TestData)[]>>.Right(Option<(string, TestData)[]>.None());

    public static readonly Either<Error, Option<(string, TestData)[]>>[] ErrorTuplesTestData =
        [
            SomeErrorTuples,
            NoneErrorTuples
        ];

    private static readonly Either<Error, Option<TestData[]>> SomeErrorOptionTestData = Either<Error, Option<TestData[]>>.Right(new[] { new TestData(1), new TestData(2) }.ToOption());
    private static readonly Either<Error, Option<TestData[]>> NoneErrorOptionTestData = Either<Error, Option<TestData[]>>.Right(Option<TestData[]>.None());

    public static readonly Either<Error, Option<TestData[]>>[] ErrorTestDataTestData =
        [
            SomeErrorOptionTestData,
            NoneErrorOptionTestData
        ];

    private static readonly Either<Error, Option<string[]>> SomeErrorOptionStringArray = Either<Error, Option<string[]>>.Right(new[] { "first", "second" }.ToOption());
    private static readonly Either<Error, Option<string[]>> NoneErrorOptionStringArray = Either<Error, Option<string[]>>.Right(Option<string[]>.None());

    public static readonly Either<Error, Option<string[]>>[] ErrorStringArrayTestData =
    [
        SomeErrorOptionStringArray,
        NoneErrorOptionStringArray
    ];

    public static readonly Either<Error, string[]>[] ErrorStringsTestData =
    [
        Either<Error, string[]>.Right(["success1", "success2"]),
        Either<Error, string[]>.Left(Error.New("error"))
    ];

    public static readonly Either<Error, long>[] ErrorLongTestData =
    [
        Either<Error, long>.Right(1),
        Either<Error, long>.Right(2),
        Either<Error, long>.Left(Error.New("error"))
    ];
}
