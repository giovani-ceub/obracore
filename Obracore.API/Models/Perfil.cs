using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class Perfil
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(45)]
        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        // Navegação
        public ICollection<UsuarioPerfil>? UsuarioPerfis { get; set; }
    }
}
