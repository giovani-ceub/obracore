using ProjectTemplate.Domain.Exceptions;
using ProjectTemplate.Domain.ValueObjects;

namespace ProjectTemplate.Domain.UnitTests.ValueObjects;

public class ColorTests
{
    [Fact]
    public void Should_ReturnCorrectColorCode()
    {
        var code = "#FFFFFF";

        var color = Color.From(code);

        color.Code.Should().Be(code);
    }

    [Fact]
    public void Should_ReturnsCode_When_ConvertToString()
    {
        var color = Color.White;

        color.ToString().Should().Be(color.Code);
    }

    [Fact]
    public void Should_PerformImplicitConversion_To_ColorCodeString()
    {
        string code = Color.White;

        code.Should().Be("#FFFFFF");
    }

    [Fact]
    public void Should_PerformExplicitConversion_When_IsSupportedColorCode()
    {
        var color = (Color)"#FFFFFF";

        color.Should().Be(Color.White);
    }

    [Fact]
    public void Should_ThrowUnsupportedColorException_When_NotSupportedColorCode()
    {
        FluentActions.Invoking(() => Color.From("##FF33CC"))
            .Should().Throw<UnsupportedColorException>();
    }
}
