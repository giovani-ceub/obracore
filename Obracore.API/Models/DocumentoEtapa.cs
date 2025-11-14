using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class DocumentoEtapa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Caminho { get; set; } = string.Empty; // Caminho no storage

        public DateTime DtCriacao { get; set; } = DateTime.Now;

        [Required]
        public int EtapaId { get; set; } // Chave estrangeira para Etapa

        [ForeignKey(nameof(EtapaId))]
        public Etapa? Etapa { get; set; }
    }
}