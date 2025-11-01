using ProjectTemplate.Application.Common.Models;
using ProjectTemplate.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using ProjectTemplate.Application.TodoLists.Queries.GetTodos;
using ProjectTemplate.Domain.Entities;

namespace ProjectTemplate.Application.Common.Mappings;

public static class TodoItemMappingExtensions
{
    public static IQueryable<TodoItemBriefDto> AsQueryableTodoItemBriefDto(this IQueryable<TodoItem> queryable)
    {
        return queryable.Select(item => new TodoItemBriefDto
        {
            Id = item.Id,
            ListId = item.ListId,
            Title = item.Title,
            Done = item.Done
        });
    }
    public static TodoItemDto AsTodoItemDto(this TodoItem item)
    {
        return new TodoItemDto
        {
            Id = item.Id,
            ListId = item.ListId,
            Title = item.Title,
            Done = item.Done,
            Priority = (int)item.Priority,
            Note = item.Note
        };
    }
    public static LookupDto AsLookupDto(this TodoItem item)
    {
        return new LookupDto
        {
            Id = item.Id,
            Title = item.Title
        };
    }

}