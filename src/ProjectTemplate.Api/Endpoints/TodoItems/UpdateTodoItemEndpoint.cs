using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectTemplate.Application.TodoItems.Commands.UpdateTodoItem;

namespace ProjectTemplate.Api.Endpoints.TodoItems
{
    public class UpdateTodoItemEndpoint : Endpoint<UpdateTodoItemCommand,
        Results<NoContent, BadRequest>>
    {
        private ISender _sender;

        public UpdateTodoItemEndpoint(ISender sender)
        {
            _sender = sender;
        }

        public override void Configure()
        {
            Put("/{id}");
            Group<TodoItemsEndpointGroup>();
        }
        public override async Task<Results<NoContent, BadRequest>> ExecuteAsync(UpdateTodoItemCommand req, CancellationToken ct)
        {
            int todoItemId = Route<int>("id");

            if (todoItemId != req.Id)
                return TypedResults.BadRequest();

            await _sender.Send(req);
            return TypedResults.NoContent();
        }
    }
}
