namespace FeedbackAutomation.AI;

public class IAService
{
    public async Task<string> GerarMensagemAsync(int nota)
    {
        await Task.Delay(100);

        return $"Olá! Percebemos sua nota {nota} e gostaríamos de entender melhor sua experiência.";
    }
}
