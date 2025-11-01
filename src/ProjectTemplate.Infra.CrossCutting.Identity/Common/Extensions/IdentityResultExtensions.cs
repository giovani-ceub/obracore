using Microsoft.AspNetCore.Identity;
using ProjectTemplate.Application.Common.Models;

namespace ProjectTemplate.Infra.CrossCutting.Identity.Common.Extensions;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}
