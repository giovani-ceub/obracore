using ProjectTemplate.Application.Common.Exceptions;
using ProjectTemplate.Application.IntegrationTests.Fixtures;
using ProjectTemplate.Application.TodoItems.Commands.CreateTodoItem;
using ProjectTemplate.Application.TodoLists.Commands.CreateTodoList;
using ProjectTemplate.Domain.Entities;

namespace ProjectTemplate.Application.IntegrationTests.TodoItems.Commands
{

    public class CreateTodoItemCommandHandlerTests : BaseIntegrationTest
    {
        public CreateTodoItemCommandHandlerTests(TestDatabaseFixture fixture, TestRedisFixture redisFixture)
            : base(fixture, redisFixture)
        {
        }

        [Fact]
        public async Task Create_GivenValidCommand_ShouldAddTodoItem()
        {
            // Arrange        
            string userId = await RunAsDefaultUserAsync();
            var listId = await Sender.Send(new CreateTodoListCommand { Title = "New List" });
            var command = new CreateTodoItemCommand { ListId = listId, Title = "Test Todo Item" };

            // Act
            int todoItemId = await Sender.Send(command, CancellationToken.None);

            // Assert            
            TodoItem? todoItem = await DbContext.TodoItems.FindAsync(todoItemId);

            todoItem.Should().NotBeNull();
            todoItem!.ListId.Should().Be(command.ListId);
            todoItem.Title.Should().Be(command.Title);
            todoItem.Done.Should().BeFalse();
            todoItem.CreatedBy.Should().Be(userId);
            todoItem.LastModifiedBy.Should().Be(userId);
            todoItem.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
            todoItem.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        }

        [Fact]
        public async Task Delete_GivenInvalidCommand_ShouldRequireMinimumFields()
        {
            // Arrange
            var command = new CreateTodoItemCommand();

            // Act & Assert
            await FluentActions.Invoking(() =>
                Sender.Send(command, CancellationToken.None)).Should().ThrowAsync<ValidationException>();
        }
    }
}
