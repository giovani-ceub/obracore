using Ardalis.GuardClauses;
using ProjectTemplate.Application.IntegrationTests.Fixtures;
using ProjectTemplate.Application.TodoItems.Commands.CreateTodoItem;
using ProjectTemplate.Application.TodoItems.Commands.UpdateTodoItemDetail;
using ProjectTemplate.Application.TodoLists.Commands.CreateTodoList;
using ProjectTemplate.Domain.Entities;
using ProjectTemplate.Domain.Enums;

namespace ProjectTemplate.Application.IntegrationTests.TodoItems.Commands
{
    public class UpdateTodoItemDetailCommandHandlerTests : BaseIntegrationTest
    {
        public UpdateTodoItemDetailCommandHandlerTests(TestDatabaseFixture fixture,
            TestRedisFixture redisFixture)
            : base(fixture, redisFixture)
        {
        }

        [Fact]
        public async Task Update_GivenInexistentId_ShouldReturnNotFoundException()
        {
            // Arrange
            var command = new UpdateTodoItemDetailCommand { Id = 99, ListId = 33, Note = "New Title", Priority = PriorityLevel.High };

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
            var command = new UpdateTodoItemDetailCommand
            {
                Id = itemId,
                ListId = listId,
                Note = "This is the note.",
                Priority = PriorityLevel.High
            };
            await Sender.Send(command, CancellationToken.None);

            // Assert
            TodoItem? item = await DbContext.TodoItems.FindAsync(itemId);

            item.Should().NotBeNull();
            item!.ListId.Should().Be(command.ListId);
            item.Note.Should().Be(command.Note);
            item.Priority.Should().Be(command.Priority);
            item.LastModifiedBy.Should().NotBeNull();
            item.LastModifiedBy.Should().Be(userId);
            item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        }

    }
}
