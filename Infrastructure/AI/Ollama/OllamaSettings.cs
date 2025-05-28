namespace Infrastructure.AI.Ollama;

public class OllamaSettings
{
    public string Host { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string EmbeddingModel { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
}