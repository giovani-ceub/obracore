using Microsoft.AspNetCore.Identity;

namespace ProjectTemplate.Infra.CrossCutting.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }   
}
