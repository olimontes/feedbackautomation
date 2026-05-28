using OpenAI;
using OpenAI.Chat;

namespace FeedbackAutomation.AI;

public class IAService
{
    private readonly string _apiKey;

    public IAService(IConfiguration configuration)
    {
        _apiKey = configuration["OpenAI:ApiKey"]!;
    }

    public async Task<string> GerarMensagemAsync(
        string nomeCliente,
        int nota
    )
    {
        var client = new ChatClient(
            model: "gpt-4.1-mini",
            apiKey: _apiKey
        );

        var prompt = $@"
O cliente {nomeCliente} deu nota {nota}/10.

Gere uma mensagem educada, humana e curta
pedindo desculpas pela experiência ruim
e tentando entender o problema.
";

        var response = await client.CompleteChatAsync(prompt);

        return response.Value.Content[0].Text;
    }
}