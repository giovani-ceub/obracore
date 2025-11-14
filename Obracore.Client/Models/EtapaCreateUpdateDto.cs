using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Obracore.Client.Models
{
    public class EtapaCreateUpdateDto
    {
        public int Id { get; set; }

        // --- Campos principais ---
        [Required(ErrorMessage = "O título da etapa é obrigatório.")]
        [MaxLength(100)]
        public string Titulo { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O ID da obra é obrigatório.")]
        public int ObraId { get; set; }

        // --- Campos obrigatórios novos ---
        [Required(ErrorMessage = "O status da etapa é obrigatório.")]
        [MaxLength(1)]
        public string Status { get; set; } = "P"; // P = Pendente, A = Ativa, C = Concluída

        [Required(ErrorMessage = "A ordem é obrigatória.")]
        public int Ordem { get; set; }

        // --- Datas ---
        public DateTime? DtInicio { get; set; }
        public DateTime? DtFimPrevista { get; set; }
        public DateTime? DtFim { get; set; }

        // --- Listas aninhadas (usam seus modelos atuais) ---
        public List<CustoEtapaModel> CustosEtapa { get; set; } = new();
        public List<DocumentoEtapaModel> DocumentosEtapa { get; set; } = new();
    }
}
