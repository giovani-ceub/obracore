using ProjectTemplate.Application.Common.Models;
using ProjectTemplate.Application.TodoLists.Queries.GetTodos;
using ProjectTemplate.Domain.Entities;

namespace ProjectTemplate.Application.Common.Mappings;

public static class TodoListMappingExtensions
{
    public static IQueryable<TodoListDto> AsQueryableTodoListDto(this IQueryable<TodoList> queryable)
    {
        return queryable.Select(list => new TodoListDto
        {
            Id = list.Id,
            Title = list.Title,
            Color = list.Color,
            Items = list.Items.Count > 0
                ? list.Items.Select(i => i.AsTodoItemDto()).ToArray()
                : Array.Empty<TodoItemDto>()
        });
    }
    public static TodoListDto AsTodoListDto(this TodoList todoList)
    {
        return new TodoListDto
        {
            Id = todoList.Id,
            Title = todoList.Title,
            Color = todoList.Color,
            Items = todoList.Items.Count > 0
                ? todoList.Items.Select(i => i.AsTodoItemDto()).ToArray()
                : Array.Empty<TodoItemDto>()
        };
    }

    public static LookupDto AsLookupDto(this TodoList list)
    {
        return new LookupDto
        {
            Id = list.Id,
            Title = list.Title
        };
    }
}
