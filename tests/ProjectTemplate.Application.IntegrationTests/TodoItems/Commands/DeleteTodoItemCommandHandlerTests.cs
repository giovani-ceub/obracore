using Ardalis.GuardClauses;
using ProjectTemplate.Application.IntegrationTests.Fixtures;
using ProjectTemplate.Application.TodoItems.Commands.CreateTodoItem;
using ProjectTemplate.Application.TodoItems.Commands.DeleteTodoItem;
using ProjectTemplate.Application.TodoLists.Commands.CreateTodoList;
using ProjectTemplate.Domain.Entities;

namespace ProjectTemplate.Application.IntegrationTests.TodoItems.Commands
{

    public class DeleteTodoItemCommandHandlerTests : BaseIntegrationTest
    {
        public DeleteTodoItemCommandHandlerTests(TestDatabaseFixture fixture, TestRedisFixture redisFixture)
            : base(fixture, redisFixture)
        {
        }

        [Fact]
        public async Task Delete_GivenInexistentId_ShouldReturnNotFoundException()
        {
            // Arrange
            var command = new DeleteTodoItemCommand(99);

            // Act & Assert
            await FluentActions.Invoking(() =>
                Sender.Send(command, CancellationToken.None)).Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task ShouldDeleteTodoItem()
        {
            // Arrange
            int listId = await Sender.Send(new CreateTodoListCommand { Title = "New List" }, CancellationToken.None);
            int todoItemId = await Sender.Send(new CreateTodoItemCommand { ListId = listId, Title = "New List" }, CancellationToken.None);

            // Act
            await Sender.Send(new DeleteTodoItemCommand(todoItemId), CancellationToken.None);

            // Assert
            TodoItem? item = await DbContext.TodoItems.FindAsync(todoItemId);
            item.Should().BeNull();
        }
    }
}
