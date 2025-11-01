using FluentAssertions;
using ProjectTemplate.Application.Common.Mappings;
using ProjectTemplate.Domain.Entities;
using ProjectTemplate.Domain.ValueObjects;
using Xunit;

namespace ProjectTemplate.Application.UnitTests.Common.Mappings;

public class MappingTests
{    
    /// <summary>
    /// Example test for specific data properties
    /// </summary>
    [Fact]
    public void Should_Map_TodoList_To_TodoListDto_Correctly()
    {
        // Arrange
        var source = new TodoList { Id = 1, Color = Color.Blue };

        // Act
        var destination = source.AsTodoListDto();

        // Assert
        // Assert that the destination has the expected values based on the source's values
        destination.Id.Should().Be(source.Id);
        destination.Color.Should().Be(source.Color.ToString());
    }

    /// <summary>
    /// Another example test for specific data properties
    /// </summary>
    [Fact]
    public void Should_Map_TodoItem_To_TodoItemDto_Correctly()
    {
        // Arrange
        var source = new TodoItem { Id = 1, Title = "Todo test case" };

        // Act
        var destination = source.AsTodoItemDto();

        // Assert
        // Assert that the destination has the expected values based on the source's values
        destination.Id.Should().Be(source.Id);
        destination.Title.Should().Be(source.Title);
    }   
}
