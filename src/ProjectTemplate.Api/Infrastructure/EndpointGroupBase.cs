namespace ProjectTemplate.Api.Infrastructure;

public abstract class EndpointGroupBase
{
    public abstract void Map(WebApplication app);
    protected virtual string GetScopesRequiredByApi(WebApplication app)
    {
        return string.Empty;
    }
}
