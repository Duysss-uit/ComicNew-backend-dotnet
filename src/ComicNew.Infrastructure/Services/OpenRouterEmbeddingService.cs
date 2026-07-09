using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ComicNew.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ComicNew.Infrastructure.Services;

public class OpenRouterEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<OpenRouterEmbeddingService> _logger;
    private const string ModelName = "nvidia/llama-nemotron-embed-vl-1b-v2:free";

    public OpenRouterEmbeddingService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenRouterEmbeddingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["OpenRouter:ApiKey"] ?? throw new InvalidOperationException("OpenRouter API Key is missing.");
        
        _httpClient.BaseAddress = new Uri("https://openrouter.ai/api/v1/");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        // OpenRouter optional headers
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:8080");
        _httpClient.DefaultRequestHeaders.Add("X-Title", "ComicNew");
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        var requestBody = new
        {
            model = ModelName,
            input = text
        };

        var response = await _httpClient.PostAsJsonAsync("embeddings", requestBody, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("OpenRouter API error: {Error}", error);
            throw new Exception($"Failed to generate embedding: {response.StatusCode} - {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<OpenRouterEmbeddingResponse>(cancellationToken: cancellationToken);
        
        if (result?.Data == null || result.Data.Count == 0)
        {
            throw new Exception("OpenRouter API returned empty embedding data.");
        }

        return result.Data[0].Embedding;
    }

    private class OpenRouterEmbeddingResponse
    {
        [JsonPropertyName("data")]
        public List<OpenRouterEmbeddingData> Data { get; set; } = new();
    }

    private class OpenRouterEmbeddingData
    {
        [JsonPropertyName("embedding")]
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}
