using System;
using System.ComponentModel.DataAnnotations;

namespace Obracore.Client.Models
{
    public class CustoEtapaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser positivo.")]
        public decimal Valor { get; set; }

        public DateTime? DtRegistro { get; set; }
        public int EtapaId { get; set; }
    }

    public class DocumentoEtapaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        public string? Titulo { get; set; }

        [Required(ErrorMessage = "O caminho é obrigatório.")]
        public string? Caminho { get; set; }

        public DateTime? DtCriacao { get; set; }
        public int EtapaId { get; set; }
    }
}
