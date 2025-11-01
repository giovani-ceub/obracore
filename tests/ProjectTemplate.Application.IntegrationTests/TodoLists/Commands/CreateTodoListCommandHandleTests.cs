using ProjectTemplate.Application.Common.Exceptions;
using ProjectTemplate.Application.IntegrationTests.Fixtures;
using ProjectTemplate.Application.TodoLists.Commands.CreateTodoList;
using ProjectTemplate.Domain.Entities;

namespace ProjectTemplate.Application.IntegrationTests.TodoLists.Commands;
public class CreateTodoListCommandHandleTests : BaseIntegrationTest
{
    public CreateTodoListCommandHandleTests(TestDatabaseFixture fixture,
        TestRedisFixture redisFixture)
        : base(fixture, redisFixture)
    {
    }

    [Fact]
    public async Task Create_GivenInvalidCommand_ShouldRequireMinimumFields()
    {
        // Arrange
        var command = new CreateTodoListCommand();

        // Action & Assert
        await FluentActions.Invoking(() => Sender.Send(command)).Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Create_GivenDuplicateTitle_ShouldRequireUniqueTitle()
    {
        // Arrange
        var command = new CreateTodoListCommand { Title = "Shopping" };

        // Action & Assert
        await Sender.Send(command);
        await FluentActions.Invoking(() =>
            Sender.Send(command)).Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Create_GivenValidCommand_ShouldCreateTodoList()
    {
        // Arrange
        string userId = await RunAsDefaultUserAsync();
        var command = new CreateTodoListCommand { Title = "Tasks" };

        // Action
        int id = await Sender.Send(command);

        // Assert
        TodoList? todoList = await DbContext.TodoLists.FindAsync(id);

        todoList.Should().NotBeNull();
        todoList!.Title.Should().Be(command.Title);
        todoList.CreatedBy.Should().Be(userId);
        todoList.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
