using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class CustoEtapa
    {
        [Key]
        public int Id { get; set; }

        public string? Descricao { get; set; }

        [Required, Column(TypeName = "decimal(12,2)")]
        public decimal Valor { get; set; }

        [Required]
        public DateTime DtRegistro { get; set; } = DateTime.UtcNow;

        [Required]
        public int EtapaId { get; set; }

        [ForeignKey(nameof(EtapaId))]
        public Etapa? Etapa { get; set; }
    }
}
