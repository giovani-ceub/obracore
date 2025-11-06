namespace Obracore.Client.Models
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }

    public class UsuarioClaims
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public List<string>? Perfis { get; set; }
    }
}
