
using OpenAI;
using OpenAI.Chat;

namespace WebTonyWilly.Services
{
    public class OpenAIService
    {
        private readonly string _apiKey;

        public OpenAIService(IConfiguration configuration)
        {
            _apiKey = configuration["OpenAI:ApiKey"];
        }

        public async Task<string> PreguntarIA(string mensaje)
        {
            var client = new OpenAIClient(_apiKey);

            var chat = client.GetChatClient("gpt-4o-mini");

            var response = await chat.CompleteChatAsync(mensaje);

            return response.Value.Content[0].Text;
        }
    }
}
