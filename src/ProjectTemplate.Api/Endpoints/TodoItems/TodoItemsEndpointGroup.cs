using FastEndpoints;

namespace ProjectTemplate.Api.Endpoints.TodoItems
{
    public class TodoItemsEndpointGroup : Group
    {
        public TodoItemsEndpointGroup()
        {
            string groupName = "Todo/Items";

            Configure($"/api/todo/items", ep =>
            {
                ep.Description(x => x
                  .RequireAuthorization()
                  .Produces(TypedResults.Unauthorized().StatusCode)
                  .WithGroupName(groupName)
                  .WithTags(groupName)
                  .WithOpenApi());
            });
        }
    }
}
