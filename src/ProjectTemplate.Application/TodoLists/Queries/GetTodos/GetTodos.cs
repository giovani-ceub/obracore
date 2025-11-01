using ProjectTemplate.Application.Abstrations.Caching;
using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Application.Common.Mappings;
using ProjectTemplate.Application.Common.Models;
using ProjectTemplate.Application.Common.Security;
using ProjectTemplate.Domain.Enums;

namespace ProjectTemplate.Application.TodoLists.Queries.GetTodos;

[Authorize]
public record GetTodosQuery : IRequest<TodosVm>;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, TodosVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cacheService;

    public GetTodosQueryHandler(IApplicationDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        var task = async () =>
        {
            var todosVm = new TodosVm
            {
                PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                    .Cast<PriorityLevel>()
                    .Select(p => new LookupDto { Id = (int)p, Title = p.ToString() })
                    .ToList(),

                Lists = await _context.TodoLists
                    .AsNoTracking()
                    .AsQueryableTodoListDto()
                    .OrderBy(t => t.Title)
                    .ToListAsync(cancellationToken)
            };

            return todosVm;
        };

        return await _cacheService.GetAsync(nameof(GetTodosQuery), task, 30, cancellationToken);
    }
}
