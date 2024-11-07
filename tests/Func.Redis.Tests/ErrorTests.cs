namespace Func.Redis.Tests;
public class ErrorTests
{
    public class CustomException(string message) : Exception
    {
        private readonly string _message = message;

        public override string Message => _message;
    }

    [TestCase("some message", "some message")]
    [TestCase("msg", "msg")]
    [TestCase("", "")]
    [TestCase(null, "")]
    public void New_WithMessage_ShouldReturnErrorWithMessage(string message, string expected) => Error.New(message).Message.Should().Be(expected);

    [TestCase("some message", "some message")]
    [TestCase("msg", "msg")]
    [TestCase("", "")]
    [TestCase(null, "")]
    public void New_WithException_ShouldReturnErrorWithMessage(string message, string expected) => Error.New(new CustomException(message)).Message.Should().Be(expected);
}
