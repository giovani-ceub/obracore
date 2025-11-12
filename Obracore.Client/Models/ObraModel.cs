using System;

namespace Obracore.Client.Models
{
    public class ObraModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime? DtInicio { get; set; }
        public DateTime? DtFimPrevista { get; set; }
        public DateTime? DtFim { get; set; }
        public DateTime DtCriacao { get; set; }
        public DateTime? DtEdicao { get; set; }
        public string? StatusObra { get; set; }
        public string? FotoCapa { get; set; }
    }
}
