using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Application.Common.Exceptions;
using ProjectTemplate.Application.Common.Security;
using ProjectTemplate.Application.IntegrationTests.Fixtures;
using ProjectTemplate.Application.TodoLists.Commands.CreateTodoList;
using ProjectTemplate.Application.TodoLists.Commands.PurgeTodoLists;

namespace ProjectTemplate.Application.IntegrationTests.TodoLists.Commands;
public class PurgeTodoListsCommandHandlerTests : BaseIntegrationTest
{
    public PurgeTodoListsCommandHandlerTests(TestDatabaseFixture fixture,
        TestRedisFixture redisFixture)
        : base(fixture, redisFixture)
    {
    }

    [Fact]
    public async Task Purge_GivenValidCommandAsAnonymousUser_ShouldDenyAnonymousUser()
    {
        // Arrange          
        var command = new PurgeTodoListsCommand();

        // Act
        var action = () => Sender.Send(command);

        // Assert
        command.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();
        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Purge_GivenValidCommandAsNonAdministratorUser_ShouldDenyNonAdministrator()
    {
        // Arrange          
        await RunAsDefaultUserAsync();
        var command = new PurgeTodoListsCommand();

        // Act        
        var action = () => Sender.Send(command);

        // Assert
        await action.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task Purge_GivenValidCommandAsAdministratorUser_ShouldAllowAdministrator()
    {
        // Arrange          
        await RunAsAdminUserAsync();
        var command = new PurgeTodoListsCommand();

        // Act        
        var action = () => Sender.Send(command);

        // Assert
        await action.Should().NotThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task Purge_GivenValidCommandAsAdministratorUser_ShouldRemoveAllLists()
    {
        // Arrange          
        await RunAsAdminUserAsync();
        await Sender.Send(new CreateTodoListCommand { Title = "New List #1" });
        await Sender.Send(new CreateTodoListCommand { Title = "New List #2" });
        await Sender.Send(new CreateTodoListCommand { Title = "New List #3" });
        await Sender.Send(new PurgeTodoListsCommand());

        // Act        
        int todoListCount = await DbContext.TodoLists.CountAsync();

        // Assert
        todoListCount.Should().Be(0);
    }
}
