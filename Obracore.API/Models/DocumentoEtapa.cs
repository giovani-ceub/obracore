using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class DocumentoEtapa
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
        public int EtapaId { get; set; }

        [ForeignKey(nameof(EtapaId))]
        public Etapa? Etapa { get; set; }
    }
}
