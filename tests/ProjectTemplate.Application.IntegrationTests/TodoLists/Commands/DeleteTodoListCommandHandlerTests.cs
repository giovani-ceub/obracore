using Ardalis.GuardClauses;
using ProjectTemplate.Application.IntegrationTests.Fixtures;
using ProjectTemplate.Application.TodoLists.Commands.CreateTodoList;
using ProjectTemplate.Application.TodoLists.Commands.DeleteTodoList;
using ProjectTemplate.Domain.Entities;

namespace ProjectTemplate.Application.IntegrationTests.TodoLists.Commands;
public class DeleteTodoListCommandHandlerTests : BaseIntegrationTest
{
    public DeleteTodoListCommandHandlerTests(TestDatabaseFixture fixture,
        TestRedisFixture redisFixture)
        : base(fixture, redisFixture)
    {
    }

    [Fact]
    public async Task Delete_GivenInexistentId_ShouldReturnNotFoundException()
    {
        // Arrange
        var command = new DeleteTodoListCommand(99);

        // Action & Assert
        await FluentActions.Invoking(() => Sender.Send(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Delete_GivenValidCommand_ShouldDeleteTodoList()
    {
        // Arrange
        int listId = await Sender.Send(new CreateTodoListCommand { Title = "New List" });

        // Action
        await Sender.Send(new DeleteTodoListCommand(listId));

        // Assert
        TodoList? list = await DbContext.TodoLists.FindAsync(listId);
        list.Should().BeNull();
    }
}
