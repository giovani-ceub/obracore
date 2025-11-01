using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectTemplate.Application.TodoLists.Commands.UpdateTodoList;

namespace ProjectTemplate.Api.Endpoints.TodoLists
{
    public class UpdateTodoListEndpoint : Endpoint<UpdateTodoListCommand, Results<NoContent, BadRequest>>
    {
        private ISender _sender;

        public UpdateTodoListEndpoint(ISender sender)
        {
            _sender = sender;
        }

        public override void Configure()
        {
            Put("{id}");
            Group<TodoListsEndpointGroup>();
        }

        public override async Task<Results<NoContent, BadRequest>> ExecuteAsync(UpdateTodoListCommand req, CancellationToken ct)
        {
            int todoListId = Route<int>("id");

            if (todoListId != req.Id)
                return TypedResults.BadRequest();

            await _sender.Send(req);
            return TypedResults.NoContent();
        }
    }
}
