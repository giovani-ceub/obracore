using FastEndpoints;
using ProjectTemplate.Application.TodoItems.Commands.CreateTodoItem;

namespace ProjectTemplate.Api.Endpoints.TodoItems
{
    public class CreateTodoItemEndpoint : Endpoint<CreateTodoItemCommand, int>
    {
        private ISender _sender;

        public CreateTodoItemEndpoint(ISender sender)
        {
            _sender = sender;
        }

        public override void Configure()
        {
            Post("/");
            Group<TodoItemsEndpointGroup>();
        }

        public override async Task HandleAsync(CreateTodoItemCommand req, CancellationToken ct)
        {
            var result =  await _sender.Send(req);
            await Send.OkAsync(result, ct);           
        }
    }
}
