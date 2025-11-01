using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectTemplate.Application.TodoItems.Commands.DeleteTodoItem;

namespace ProjectTemplate.Api.Endpoints.TodoItems
{
    public class DeleteTodoItemEndpoint : EndpointWithoutRequest<Results<NoContent, BadRequest>>
    {
        private ISender _sender;

        public DeleteTodoItemEndpoint(ISender sender)
        {
            _sender = sender;
        }

        public override void Configure()
        {
            Delete("/{id}");
            Group<TodoItemsEndpointGroup>();
        }
        public override async Task<Results<NoContent, BadRequest>> ExecuteAsync(CancellationToken ct)
        {
            int todoItemId = Route<int>("id");
            await _sender.Send(new DeleteTodoItemCommand(todoItemId));
            return TypedResults.NoContent();
        }
    }
}
