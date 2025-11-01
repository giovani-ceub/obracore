namespace ProjectTemplate.Application.Abstrations.Apic.Authentication
{
    public interface IAuthenticationService
    {
        Task<GetAccessTokenResponseDto> GetAccessTokenAsync();
    }
}
