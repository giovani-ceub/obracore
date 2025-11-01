using Ardalis.GuardClauses;
using ProjectTemplate.Application.Common.Exceptions;
using ProjectTemplate.Application.IntegrationTests.Fixtures;
using ProjectTemplate.Application.TodoLists.Commands.CreateTodoList;
using ProjectTemplate.Application.TodoLists.Commands.UpdateTodoList;
using ProjectTemplate.Domain.Entities;

namespace ProjectTemplate.Application.IntegrationTests.TodoLists.Commands;
public class UpdateTodoListCommandHandlerTests : BaseIntegrationTest
{
    public UpdateTodoListCommandHandlerTests(TestDatabaseFixture databaseFixture, TestRedisFixture redisFixture)
        : base(databaseFixture, redisFixture)
    {
    }

    [Fact]
    public async Task Update_GivenInexistentId_ShouldReturnNotFoundException()
    {
        // Arrange
        var command = new UpdateTodoListCommand { Id = 99, Title = "New Title" };

        // Action & Assert
        await FluentActions.Invoking(() => Sender.Send(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Update_GivenDuplicateTitle_ShouldRequireUniqueTitle()
    {
        // Arrange
        int listId = await Sender.Send(new CreateTodoListCommand { Title = "New List" });

        // Action
        await Sender.Send(new CreateTodoListCommand { Title = "Other List" });
        var command = new UpdateTodoListCommand { Id = listId, Title = "Other List" };

        // Assert
        (await FluentActions.Invoking(() =>
            Sender.Send(command))
                .Should().ThrowAsync<ValidationException>().Where(ex => ex.Errors.ContainsKey("Title")))
                .And
                .Errors["Title"].Should().Contain("'Title' must be unique.");
    }

    [Fact]
    public async Task Update_GivenValidCommandAsAuthenticatedUser_ShouldUpdateTodoList()
    {
        // Arrange
        string userId = await RunAsDefaultUserAsync();

        int listId = await Sender.Send(new CreateTodoListCommand { Title = "New List" });
        var command = new UpdateTodoListCommand { Id = listId, Title = "Updated List Title" };

        // Action
        await Sender.Send(command);

        // Assert
        TodoList? list = await DbContext.TodoLists.FindAsync(listId);

        list.Should().NotBeNull();
        list!.Title.Should().Be(command.Title);
        list.LastModifiedBy.Should().NotBeNull();
        list.LastModifiedBy.Should().Be(userId);
        list.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
