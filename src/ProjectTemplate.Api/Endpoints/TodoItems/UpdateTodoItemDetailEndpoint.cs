using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectTemplate.Application.TodoItems.Commands.UpdateTodoItemDetail;

namespace ProjectTemplate.Api.Endpoints.TodoItems
{
    public class UpdateTodoItemDetailEndpoint : Endpoint<UpdateTodoItemDetailCommand,
        Results<NoContent, BadRequest>>
    {
        private ISender _sender;

        public UpdateTodoItemDetailEndpoint(ISender sender)
        {
            _sender = sender;
        }

        public override void Configure()
        {
            Put("/detail/{id}");
            Group<TodoItemsEndpointGroup>();
        }
        public override async Task<Results<NoContent, BadRequest>> ExecuteAsync(UpdateTodoItemDetailCommand req, CancellationToken ct)
        {
            int todoItemId = Route<int>("id");

            if (todoItemId != req.Id)
                return TypedResults.BadRequest();

            await _sender.Send(req);
            return TypedResults.NoContent();
        }
    }
}
