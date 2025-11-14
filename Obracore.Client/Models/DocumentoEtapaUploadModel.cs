using Microsoft.AspNetCore.Components.Forms;

namespace Obracore.Client.Models
{
    public class DocumentoEtapaUploadModel
    {
        public string Titulo { get; set; } = string.Empty;

        // IBrowserFile vem do namespace Microsoft.AspNetCore.Components.Forms
        public IBrowserFile? Arquivo { get; set; }
    }
}
