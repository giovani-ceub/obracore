using System.ComponentModel.DataAnnotations;

namespace Obracore.Client.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string? Email { get; set; }

        // Campo para senha, opcional na edição, obrigatório na criação
        [DataType(DataType.Password)]
        public string? Senha { get; set; }

        public string? Status { get; set; }

        // Relacionamento N:N (UsuarioPerfil)
        // Armazena apenas os IDs dos perfis selecionados no formulário.
        public List<int> PerfilIds { get; set; } = new List<int>();

        // Propriedade para exibir perfis na lista, mas não para envio/recebimento CRUD
        public string? PerfisDisplay { get; set; } 
    }
}