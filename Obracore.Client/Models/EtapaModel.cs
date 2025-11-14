using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Obracore.Client.Models
{
    public class EtapaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título da etapa é obrigatório.")]
        public string? Titulo { get; set; }

        public string? Descricao { get; set; }

        // Propriedades adicionadas para alinhar com o backend
        // Status: "P" (Pendente), "A" (Ativa), "C" (Concluída) etc.
        public string? Status { get; set; }

        // Ordem para ordenar as etapas
        public int Ordem { get; set; }

        // Datas (opcionais)
        public DateTime? DtInicio { get; set; }
        public DateTime? DtFimPrevista { get; set; }
        public DateTime? DtFim { get; set; }

        // Propriedades de navegação (listas aninhadas)
        public List<CustoEtapaModel> CustosEtapa { get; set; } = new();
        public List<DocumentoEtapaModel> DocumentosEtapa { get; set; } = new();

        // FK para Obra
        public int ObraId { get; set; }
    }
}
