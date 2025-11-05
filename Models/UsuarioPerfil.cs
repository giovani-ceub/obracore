using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class UsuarioPerfil
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PerfilId { get; set; }

        [ForeignKey(nameof(PerfilId))]
        public Perfil? Perfil { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }
    }
}
