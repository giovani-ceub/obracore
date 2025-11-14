using Obracore.Client.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Obracore.Client.Services
{
    public class EtapaService
    {
        private readonly HttpClient _http;
        private readonly ToastService _toastService;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public EtapaService(HttpClient http, ToastService toastService)
        {
            _http = http;
            _toastService = toastService;
        }

        private const string BaseApi = "api/Etapas";

        // ============================
        // GET ALL
        // ============================
        public async Task<List<EtapaModel>?> GetEtapasAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<EtapaModel>>(BaseApi);
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Erro ao carregar etapas: {ex.Message}");
                return null;
            }
        }

        // ============================
        // GET BY ID
        // ============================
        public async Task<EtapaModel?> GetEtapaByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<EtapaModel>($"{BaseApi}/{id}");
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Erro ao carregar etapa {id}: {ex.Message}");
                return null;
            }
        }

        // ================================================
        // POST (CRIAR ETAPA - sem arquivos)
        // ================================================
        public async Task<int> CreateEtapaAsync(EtapaCreateUpdateDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(BaseApi, dto, _jsonOptions);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _toastService.ShowError($"Falha ao criar etapa.\n{error}");
                    return 0;
                }

                // Ler ID da etapa criada
                var created = await response.Content.ReadFromJsonAsync<EtapaModel>();

                return created?.Id ?? 0;
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Erro de comunicação: {ex.Message}");
                return 0;
            }
        }

        // ================================================
        // UPLOAD DE DOCUMENTOS
        // ================================================
        public async Task<bool> UploadDocumentosEtapa(int etapaId, EtapaUploadModel model)
        {
            try
            {
                var content = new MultipartFormDataContent();

                // Campos simples precisam acompanhar o upload
                content.Add(new StringContent(model.Nome), "Nome");
                content.Add(new StringContent(model.Descricao ?? ""), "Descricao");
                content.Add(new StringContent(model.ObraId.ToString()), "ObraId");
                content.Add(new StringContent(model.Status), "Status");
                content.Add(new StringContent(model.Ordem.ToString()), "Ordem");

                // Arquivos
                foreach (var doc in model.Documentos)
                {
                    if (doc.Arquivo != null)
                    {
                        var fileContent = new StreamContent(doc.Arquivo.OpenReadStream(20_000_000));
                        fileContent.Headers.ContentType =
                            new System.Net.Http.Headers.MediaTypeHeaderValue(doc.Arquivo.ContentType);

                        // Nome IMPORTANTE: DOCUMENTOS
                        content.Add(fileContent, "Documentos", doc.Arquivo.Name);
                    }
                }

                var response = await _http.PostAsync($"{BaseApi}/{etapaId}/upload", content);

                if (response.IsSuccessStatusCode)
                    return true;

                var error = await response.Content.ReadAsStringAsync();
                _toastService.ShowError($"Falha no upload.\n{error}");
                return false;
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Erro de upload: {ex.Message}");
                return false;
            }
        }

        // ================================================
        // UPDATE
        // ================================================
        public async Task<bool> UpdateEtapaAsync(int id, EtapaCreateUpdateDto dto)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"{BaseApi}/{id}", dto, _jsonOptions);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _toastService.ShowError($"Falha ao atualizar etapa.\n{error}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Erro ao atualizar etapa: {ex.Message}");
                return false;
            }
        }

        // ================================================
        // DELETE
        // ================================================
        public async Task<bool> DeleteEtapaAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"{BaseApi}/{id}");

                if (response.IsSuccessStatusCode)
                    return true;

                var error = await response.Content.ReadAsStringAsync();
                _toastService.ShowError($"Falha ao excluir etapa.\n{error}");
                return false;
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"Erro ao excluir etapa: {ex.Message}");
                return false;
            }
        }
    }
}
