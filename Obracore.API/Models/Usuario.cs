using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Senha { get; set; } = string.Empty;

        [Required, MaxLength(1), Column(TypeName = "char(1)")]
        public string Status { get; set; } = "A"; // ex: A=ativo, I=inativo

        [Required]
        public DateTime DtCriacao { get; set; } = DateTime.UtcNow;

        public DateTime? DtEdicao { get; set; }

        // Navegação
        public ICollection<UsuarioPerfil>? UsuarioPerfis { get; set; }
        public ICollection<UsuarioObra>? UsuariosObras { get; set; }
    }
}
