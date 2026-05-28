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
        int nota,
        string? contextoFeedback = null
    )
    {
        var client = new ChatClient(
            model: "gpt-4.1-mini",
            apiKey: _apiKey
        );

        var contexto = string.IsNullOrWhiteSpace(contextoFeedback)
            ? "Nao informado"
            : contextoFeedback.Trim();

        var instrucaoPorNota = ObterInstrucaoPorNota(nota);

        var prompt = $@"
Voce e um atendente da Automatiza Sistemas falando pelo WhatsApp.

Dados do feedback:
- Cliente ou responsavel: {nomeCliente}
- Nota recebida: {nota}/10
- Contexto informado: {contexto}

Tom da mensagem:
{instrucaoPorNota}

Regras:
- Escreva em portugues do Brasil.
- Seja educado, humano e direto.
- Use no maximo 2 frases curtas.
- Nao use markdown.
- Nao use emoji.
- Nao diga que e uma IA.
- Nao invente informacoes que nao estao nos dados.
- Termine com uma pergunta simples para continuar a conversa.

Retorne somente o texto final da mensagem.
";

        var response = await client.CompleteChatAsync(prompt);

        return response.Value.Content[0].Text.Trim();
    }

    public async Task<string> GerarRespostaWhatsAppAsync(
        string nomeCliente,
        string mensagemCliente,
        int? nota = null,
        string? contextoFeedback = null
    )
    {
        var client = new ChatClient(
            model: "gpt-4.1-mini",
            apiKey: _apiKey
        );

        var notaTexto = nota.HasValue
            ? $"{nota.Value}/10"
            : "Nao informada";

        var contexto = string.IsNullOrWhiteSpace(contextoFeedback)
            ? "Nao informado"
            : contextoFeedback.Trim();

        var prompt = $@"
Voce e um atendente da Automatiza Sistemas respondendo uma conversa no WhatsApp.

Dados conhecidos:
- Cliente ou responsavel: {nomeCliente}
- Ultima nota registrada: {notaTexto}
- Contexto do ultimo feedback: {contexto}
- Mensagem recebida do cliente: {mensagemCliente}

Regras:
- Escreva em portugues do Brasil.
- Seja educado, humano, objetivo e profissional.
- Use no maximo 2 frases curtas.
- Se houver reclamacao, reconheca o problema e diga que vamos acompanhar com atencao.
- Se a resposta for positiva, agradeca e pergunte se podemos ajudar em algo mais.
- Nao use markdown.
- Nao use emoji.
- Nao diga que e uma IA.
- Nao prometa prazos, descontos ou acoes que nao estejam nos dados.

Retorne somente o texto final da resposta.
";

        var response = await client.CompleteChatAsync(prompt);

        return response.Value.Content[0].Text.Trim();
    }

    private static string ObterInstrucaoPorNota(int nota)
    {
        if (nota <= 3)
        {
            return "Nota baixa: seja mais empatico, peca desculpas pela experiencia ruim e pergunte como podemos melhorar.";
        }

        if (nota <= 6)
        {
            return "Nota media: seja amigavel, reconheca que ainda podemos melhorar e pergunte o que tornaria a experiencia melhor.";
        }

        return "Nota alta: agradeca a avaliacao e pergunte se existe algo que podemos fazer para manter ou melhorar a experiencia.";
    }
}
