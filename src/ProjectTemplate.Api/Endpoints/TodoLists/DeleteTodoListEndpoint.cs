using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using ProjectTemplate.Application.TodoLists.Commands.DeleteTodoList;

namespace ProjectTemplate.Api.Endpoints.TodoLists
{
    public class DeleteTodoListEndpoint : EndpointWithoutRequest<Results<NoContent, BadRequest>>
    {
        private readonly ISender _sender;
        public DeleteTodoListEndpoint(ISender sender)
        {
            _sender = sender;
        }

        public override void Configure()
        {
            Delete("/{id}");
            Group<TodoListsEndpointGroup>();
        }

        public override async Task<Results<NoContent, BadRequest>> ExecuteAsync(CancellationToken ct)
        {
            int todoListId = Route<int>("id");
            await _sender.Send(new DeleteTodoListCommand(todoListId));
            return TypedResults.NoContent();
        }
    }
}
