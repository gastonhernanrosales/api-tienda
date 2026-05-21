using System.Text;
using System.Text.Json;

namespace WebTonyWilly.Services
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAIService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        }

        public async Task<string> PreguntarIA(string mensaje)
        {
            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {_apiKey}");

            var body = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = mensaje
                    }
                },
                max_tokens = 200
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                content
            );

            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine(json);

            using var doc = JsonDocument.Parse(json);

            // 🔥 Si OpenAI devuelve error
            if (!response.IsSuccessStatusCode)
            {
                return $"Error OpenAI: {json}";
            }

            // 🔥 Verificar si existe choices
            if (!doc.RootElement.TryGetProperty("choices", out var choices))
            {
                return "La IA no devolvió respuestas.";
            }

            return choices[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
    }
}
