using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class CustoEtapa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Descricao { get; set; } = string.Empty;

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        public DateTime DtRegistro { get; set; } = DateTime.Now;

        [Required]
        public int EtapaId { get; set; } // Chave estrangeira para Etapa

        [ForeignKey(nameof(EtapaId))]
        public Etapa? Etapa { get; set; }
    }
}