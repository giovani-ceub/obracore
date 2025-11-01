using ProjectTemplate.Application.Common.Exceptions;
using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Application.Common.Security;
using System.Reflection;

namespace ProjectTemplate.Application.Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public AuthorizationBehavior(
        IUser user,
        IIdentityService identityService)
    {
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        await EnsureUserIsAuthorized(authorizeAttributes);

        return await next();
    }

    private async Task EnsureUserIsAuthorized(IEnumerable<AuthorizeAttribute> authorizeAttributes)
    {
        if (!authorizeAttributes.Any())
            return; // User is authorized / authorization not required

        EnsureAuthenticatedUser();

        await EnsureUserHasRequiredRole(authorizeAttributes);
        await EnsureUserMeetsPolicyRequirements(authorizeAttributes);
    }

    private void EnsureAuthenticatedUser()
    {
        if (_user.Id == null) throw new UnauthorizedAccessException("User must be authenticated.");
    }

    private async Task EnsureUserHasRequiredRole(IEnumerable<AuthorizeAttribute> authorizeAttributes)
    {
        IEnumerable<AuthorizeAttribute> authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

        if (authorizeAttributesWithRoles.Any())
        {
            foreach (var attribute in authorizeAttributesWithRoles)
            {
                string[] roles = attribute.Roles.Split(',');
                bool userHasRole = await UserHasAnyRole(roles);

                if (userHasRole)
                {
                    return;
                }
            }

            throw new ForbiddenAccessException("User does not have the required role.");
        }
    }

    private async Task<bool> UserHasAnyRole(IEnumerable<string> roles)
    {
        foreach (string role in roles)
        {
            bool isInRole = await _identityService.IsInRoleAsync(_user.Id, role.Trim());
            if (isInRole)
            {
                return true;
            }
        }

        return false;
    }

    private async Task EnsureUserMeetsPolicyRequirements(IEnumerable<AuthorizeAttribute> authorizeAttributes)
    {
        IEnumerable<AuthorizeAttribute> authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));

        foreach (AuthorizeAttribute attribute in authorizeAttributesWithPolicies)
        {
            bool authorized = await _identityService.AuthorizeAsync(_user.Id, attribute.Policy);
            if (!authorized)
            {
                throw new ForbiddenAccessException("User does not meet policy requirements.");
            }
        }
    }
}
