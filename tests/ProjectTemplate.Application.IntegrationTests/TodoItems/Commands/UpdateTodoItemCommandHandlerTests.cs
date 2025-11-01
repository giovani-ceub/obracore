using Ardalis.GuardClauses;
using ProjectTemplate.Application.IntegrationTests.Fixtures;
using ProjectTemplate.Application.TodoItems.Commands.CreateTodoItem;
using ProjectTemplate.Application.TodoItems.Commands.UpdateTodoItem;
using ProjectTemplate.Application.TodoLists.Commands.CreateTodoList;

namespace ProjectTemplate.Application.IntegrationTests.TodoItems.Commands
{
    public class UpdateTodoItemCommandHandlerTests : BaseIntegrationTest
    {
        public UpdateTodoItemCommandHandlerTests(TestDatabaseFixture fixture,
            TestRedisFixture redisFixture)
            : base(fixture, redisFixture)
        {
        }

        [Fact]
        public async Task Update_GivenInexistentId_ShouldReturnNotFoundException()
        {
            // Arrange
            var command = new UpdateTodoItemCommand { Id = 99, Title = "New Title" };

            // Act & Assert
            await FluentActions.Invoking(() =>
                Sender.Send(command, CancellationToken.None)).Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Create_GivenValidCommand_ShouldUpdateTodoItem()
        {
            // Arrange
            string userId = await RunAsDefaultUserAsync();

            // Act
            int listId = await Sender.Send(new CreateTodoListCommand { Title = "New List" }, CancellationToken.None);
            int itemId = await Sender.Send(new CreateTodoItemCommand { ListId = listId, Title = "New Item" }, CancellationToken.None);
            var command = new UpdateTodoItemCommand
            {
                Id = itemId,
                Title = "Updated Item Title"
            };
            await Sender.Send(command, CancellationToken.None);

            // Assert
            var todoItem = await DbContext.TodoItems.FindAsync(itemId);

            todoItem.Should().NotBeNull();
            todoItem!.Title.Should().Be(command.Title);
            todoItem.LastModifiedBy.Should().NotBeNull();
            todoItem.LastModifiedBy.Should().Be(userId);
            todoItem.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        }
    }
}
