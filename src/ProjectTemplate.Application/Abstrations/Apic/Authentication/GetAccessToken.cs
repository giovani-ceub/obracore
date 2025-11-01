namespace ProjectTemplate.Application.Abstrations.Apic.Authentication
{
    public record GetAccessTokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string ExpiresIn { get; set; }
        public required string SessionState { get; set; }
    }
}
