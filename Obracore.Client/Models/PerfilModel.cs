namespace Obracore.Client.Models
{
    public class PerfilModel
    {
        public int Id { get; set; }
        
        // Nome do Perfil (ex: Admin, Usuario, Cliente)
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
    }
}