using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class CustoObra
    {
        [Key]
        public int Id { get; set; }

        public string? Descricao { get; set; }

        [Required, Column(TypeName = "decimal(12,2)")]
        public decimal Valor { get; set; }

        [Required]
        public DateTime DtRegistro { get; set; } = DateTime.UtcNow;

        public int? EtapaId { get; set; }

        [ForeignKey(nameof(EtapaId))]
        public Etapa? Etapa { get; set; }

        [Required]
        public int ObraId { get; set; }

        [ForeignKey(nameof(ObraId))]
        public Obra? Obra { get; set; }
    }
}
