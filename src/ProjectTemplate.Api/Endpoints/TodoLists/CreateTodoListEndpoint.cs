using FastEndpoints;
using ProjectTemplate.Application.TodoLists.Commands.CreateTodoList;

namespace ProjectTemplate.Api.Endpoints.TodoLists
{
    public class CreateTodoListEndpoint : Endpoint<CreateTodoListCommand, int>
    {
        private readonly ISender _sender;

        public CreateTodoListEndpoint(ISender sender)
        {
            _sender = sender;
        }

        public override void Configure()
        {
            Post("");
            Group<TodoListsEndpointGroup>();
        }

        public override async Task HandleAsync(CreateTodoListCommand req, CancellationToken ct)
        {
            var result = await _sender.Send(req);
            await Send.OkAsync(result, ct);
        }
    }
}
