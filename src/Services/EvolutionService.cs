using FeedbackAutomation.Utils;
using System.Text;
using System.Text.Json;

public class EvolutionService
{
    private readonly HttpClient _httpClient;

    public EvolutionService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.BaseAddress = new Uri("http://192.168.0.15:8080");

        _httpClient.DefaultRequestHeaders.Add("apikey", "123456");
    }

    public async Task SendMessage(string number, string text)
    {
        var telefone = TelefoneHelper.NormalizarTelefoneWhatsApp(number);

        if (!TelefoneHelper.TelefoneValidoParaWhatsApp(telefone))
        {
            throw new ArgumentException(
                "Telefone invalido para WhatsApp.",
                nameof(number)
            );
        }

        var payload = new
        {
            number = telefone,
            text
        };

        var json = JsonSerializer.Serialize(payload);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(
            "/message/sendText/feedback-bot",
            content
        );

        var result = await response.Content.ReadAsStringAsync();

        Console.WriteLine(result);

        response.EnsureSuccessStatusCode();
    }
}
