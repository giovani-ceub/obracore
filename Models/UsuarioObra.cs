using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class UsuarioObra
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ObraId { get; set; }

        [ForeignKey(nameof(ObraId))]
        public Obra? Obra { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }
    }
}
