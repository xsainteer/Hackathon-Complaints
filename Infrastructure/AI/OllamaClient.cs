using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Infrastructure.AI;

public class OllamaClient
{
    private readonly OllamaSettings _ollamaSettings;
    private readonly HttpClient _httpClient;

    public OllamaClient(IOptions<OllamaSettings> ollamaSettings, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _ollamaSettings = ollamaSettings.Value;
    }

    public async Task<string?> MakeShortDescriptionAsync(string description)
    {
        var payload = new
        {
            model = _ollamaSettings.Model,
            prompt = $"{description}" +
                     $"\nПрочитай текст ввыше, выделенный кавычками." +
                     $"\nЕсли он содержит бред, угрозы, агрессию, истерику, нелогичные" +
                     $" обвинения, бессвязные фразы или эмоциональную неадекватность —" +
                     $" верни только слово “Неадекватно”.\n\nЕсли жалоба написана " +
                     $"спокойно, логично, с конкретными проблемами и без истерики " +
                     $"— верни краткий пересказ (до 30 слов), без эмоций, по сути." +
                     $"\n\nНе добавляй объяснений. Ответ должен быть строго одним из " +
                     $"двух: либо \"Неадекватно\", либо краткий пересказ.",
            stream = false
        };
        
        var json = JsonSerializer.Serialize(payload);
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_ollamaSettings.Port}/api/generate", content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        
        using var doc = JsonDocument.Parse(responseString);
        
        return doc.RootElement.GetProperty("response").GetString();
    }
}