using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class DocumentoObra
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Caminho { get; set; } = string.Empty;

        [Required]
        public DateTime DtCriacao { get; set; } = DateTime.UtcNow;

        [Required]
        public int ObraId { get; set; }

        [ForeignKey(nameof(ObraId))]
        public Obra? Obra { get; set; }
    }
}
