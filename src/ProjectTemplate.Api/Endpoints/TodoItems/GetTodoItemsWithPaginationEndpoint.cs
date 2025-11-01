using FastEndpoints;
using ProjectTemplate.Application.Common.Models;
using ProjectTemplate.Application.TodoItems.Queries.GetTodoItemsWithPagination;

namespace ProjectTemplate.Api.Endpoints.TodoItems
{
    public class GetTodoItemsWithPaginationEndpoint : Endpoint<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>
    {
        private ISender _sender;

        public GetTodoItemsWithPaginationEndpoint(ISender sender)
        {
            _sender = sender;
        }

        public override void Configure()
        {
            Get("/");
            Group<TodoItemsEndpointGroup>();
        }

        public override async Task HandleAsync([AsParameters] GetTodoItemsWithPaginationQuery req, CancellationToken ct)
        {
            var result = await _sender.Send(req);
            await Send.OkAsync(result, ct);
        }
    }
}
