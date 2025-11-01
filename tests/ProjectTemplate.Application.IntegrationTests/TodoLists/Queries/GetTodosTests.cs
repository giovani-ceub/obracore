using ProjectTemplate.Application.IntegrationTests.Fixtures;
using ProjectTemplate.Application.TodoLists.Queries.GetTodos;
using ProjectTemplate.Domain.Entities;
using ProjectTemplate.Domain.ValueObjects;


namespace ProjectTemplate.Application.IntegrationTests.TodoLists.Queries;
public class GetTodosTests : BaseIntegrationTest
{
    public GetTodosTests(TestDatabaseFixture fixture,
        TestRedisFixture redisFixture)
        : base(fixture, redisFixture)
    {
    }

    [Fact]
    public async Task Get_GivenValidCommandWithoutAnyListsAndItems_ShouldReturnPriorityLevels()
    {
        // Arrange
        await RunAsDefaultUserAsync();
        var query = new GetTodosQuery();

        // Action
        TodosVm? result = await Sender.Send(query);

        // Assert
        result.PriorityLevels.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Get_GivenValidCommandAsAuthenticatedUser_ShouldReturnAllListsAndItems()
    {
        // Arrange
        await RunAsDefaultUserAsync();
        var todoList = new TodoList
        {
            Title = "Shopping",
            Color = Color.Blue,
            Items =
                    {
                        new TodoItem { Title = "Apples", Done = true },
                        new TodoItem { Title = "Milk", Done = true },
                        new TodoItem { Title = "Bread", Done = true },
                        new TodoItem { Title = "Toilet paper" },
                        new TodoItem { Title = "Pasta" },
                        new TodoItem { Title = "Tissues" },
                        new TodoItem { Title = "Tuna" }
                    }
        };
        await DbContext.TodoLists.AddAsync(todoList);
        await DbContext.SaveChangesAsync();

        var query = new GetTodosQuery();

        // Action
        TodosVm? result = await Sender.Send(query);

        // Assert
        result.Lists.Should().HaveCount(2);
        result.Lists.First().Items.Should().HaveCount(todoList.Items.Count());
    }

    [Fact]
    public async Task Get_GivenValidCommandAsAnonymousUser_ShouldDenyAnonymousUser()
    {
        // Arrange
        var query = new GetTodosQuery();

        // Action
        var action = () => Sender.Send(query);

        // Assert
        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
