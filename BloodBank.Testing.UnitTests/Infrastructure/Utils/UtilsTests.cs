using BloodBank.Infrastructure;

namespace BloodBank.Testing.UnitTests.Infrastructure.Utils;

public class UtilsTests
{
    [Theory]
    [InlineData("MyTestString", "my-test-string")]
    [InlineData("BloodBank", "blood-bank")]
    [InlineData("A", "a")]
    [InlineData("HTMLParser", "h-t-m-l-parser")]
    [InlineData("Already-dashed", "already-dashed")]
    public void InputString_ToDashCaseIsCalled_ItShouldConvertCorrectly(string input, string expected)
    {
        var result = input.ToDashCase();

        Assert.Equal(result, expected);
    }

    [Fact]
    public void NullInput_ToDashCaseIsCalled_ItShouldThrowArgumentNullException()
    {
        string input = null!;
        var act = () => input.ToDashCase();
        Assert.Throws<ArgumentNullException>(act);
    }
}