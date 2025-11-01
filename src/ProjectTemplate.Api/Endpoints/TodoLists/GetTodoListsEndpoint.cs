using FastEndpoints;
using ProjectTemplate.Application.TodoLists.Queries.GetTodos;

namespace ProjectTemplate.Api.Endpoints.TodoLists
{
    public class GetTodoListsEndpoint : EndpointWithoutRequest<TodosVm>
    {
        private readonly ISender _sender;
        public GetTodoListsEndpoint(ISender sender)
        {
            _sender = sender;
        }

        public override void Configure()
        {
            Get("/");
            Group<TodoListsEndpointGroup>();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = await _sender.Send(new GetTodosQuery(), ct);
            await Send.OkAsync(result, ct);
        }
    }
}
