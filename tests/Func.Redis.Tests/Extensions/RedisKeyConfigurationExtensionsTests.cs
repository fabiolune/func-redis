using Func.Redis.Extensions;

namespace Func.Redis.Tests.Extensions;

internal class RedisKeyConfigurationExtensionsTests
{
    [TestCase("key")]
    [TestCase("another-key")]
    [TestCase("prefix:key")]
    public void GetKeyMapper_WhenConfigIsNull_ShouldReturnIdentityFunction(string input)
    {
        var config = null as RedisKeyConfiguration;
        var mapper = config.GetKeyMapper();
        var inverseMapper = config.GetInverseKeyMapper();

        mapper(input).Should().Be(input);
        inverseMapper(input).Should().Be(input);
    }

    [TestCase("key")]
    [TestCase("another-key")]
    [TestCase("prefix:key")]
    public void GetKeyMapper_WhenKeyPrefixIsNull_ShouldReturnIdentityFunction(string input)
    {
        var config = new RedisKeyConfiguration();
        var mapper = config.GetKeyMapper();
        var inverseMapper = config.GetInverseKeyMapper();

        mapper(input).Should().Be(input);
        inverseMapper(input).Should().Be(input);
    }

    [TestCase("", "key")]
    [TestCase("", "another-key")]
    [TestCase("", "prefix:key")]
    [TestCase(" ", "key")]
    [TestCase(" ", "another-key")]
    [TestCase(" ", "prefix:key")]
    [TestCase("  ", "key")]
    [TestCase("  ", "another-key")]
    [TestCase("  ", "prefix:key")]
    [TestCase(":", "key")]
    [TestCase(":", "another-key")]
    [TestCase(":", "prefix:key")]
    [TestCase(" :", "key")]
    [TestCase(" :", "another-key")]
    [TestCase(" :", "prefix:key")]
    [TestCase(": ", "key")]
    [TestCase(": ", "another-key")]
    [TestCase(": ", "prefix:key")]
    [TestCase(": :", "key")]
    [TestCase(": :", "another-key")]
    [TestCase(": :", "prefix:key")]
    [TestCase(" : ", "key")]
    [TestCase(" : ", "another-key")]
    [TestCase(" : ", "prefix:key")]
    public void GetKeyMapper_WhenKeyPrefixIsEmptyOrCommas_ShouldReturnIdentityFunction(string prefix, string input)
    {
        var mapper = new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        }.GetKeyMapper();

        var inverseMapper = new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        }.GetInverseKeyMapper();

        mapper(input).Should().Be(input);
        inverseMapper(input).Should().Be(input);
    }

    [TestCase("prefix", "key", "prefix:key")]
    [TestCase("prefix", "another-key", "prefix:another-key")]
    [TestCase("prefix", "prefixed:key", "prefix:prefixed:key")]

    [TestCase("prefix:", "key", "prefix:key")]
    [TestCase("prefix:", "another-key", "prefix:another-key")]
    [TestCase("prefix:", "prefixed:key", "prefix:prefixed:key")]

    [TestCase("prefix::", "key", "prefix:key")]
    [TestCase("prefix::", "another-key", "prefix:another-key")]
    [TestCase("prefix::", "prefixed:key", "prefix:prefixed:key")]

    [TestCase(":prefix", "prefixed:key", "prefix:prefixed:key")]
    [TestCase(":prefix", "key", "prefix:key")]
    [TestCase(":prefix", "another-key", "prefix:another-key")]

    [TestCase("::prefix", "key", "prefix:key")]
    [TestCase("::prefix", "another-key", "prefix:another-key")]
    [TestCase("::prefix", "prefixed:key", "prefix:prefixed:key")]

    [TestCase(":prefix:", "prefixed:key", "prefix:prefixed:key")]
    [TestCase(":prefix:", "key", "prefix:key")]
    [TestCase(":prefix:", "another-key", "prefix:another-key")]

    [TestCase("prefix ", "key", "prefix:key")]
    [TestCase("prefix ", "another-key", "prefix:another-key")]
    [TestCase("prefix ", "prefixed:key", "prefix:prefixed:key")]

    [TestCase("prefix  ", "key", "prefix:key")]
    [TestCase("prefix  ", "another-key", "prefix:another-key")]
    [TestCase("prefix  ", "prefixed:key", "prefix:prefixed:key")]

    [TestCase(" prefix", "prefixed:key", "prefix:prefixed:key")]
    [TestCase(" prefix", "key", "prefix:key")]
    [TestCase(" prefix", "another-key", "prefix:another-key")]

    [TestCase("  prefix", "key", "prefix:key")]
    [TestCase("  prefix", "another-key", "prefix:another-key")]
    [TestCase("  prefix", "prefixed:key", "prefix:prefixed:key")]

    [TestCase(" prefix ", "prefixed:key", "prefix:prefixed:key")]
    [TestCase(" prefix ", "key", "prefix:key")]
    [TestCase(" prefix ", "another-key", "prefix:another-key")]

    public void GetKeyMapper_WhenPrefixIsNotEmpty_ShouldReturnPrefixedKey(string prefix, string input, string output)
    {
        var mapper = new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        }.GetKeyMapper();
        var inverseMapper = new RedisKeyConfiguration
        {
            KeyPrefix = prefix
        }.GetInverseKeyMapper();

        mapper(input).Should().Be(output);
        inverseMapper(output).Should().Be(input);
    }
}