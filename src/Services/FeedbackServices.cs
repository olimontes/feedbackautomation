// src/Services/FeedbackService.cs

using FeedbackAutomation.Entities;
using FeedbackAutomation.AI;

namespace FeedbackAutomation.Services;

public class FeedbackService
{
    private readonly IAService _iaService;

    public FeedbackService(IAService iaService)
    {
        _iaService = iaService;
    }

    public async Task ProcessarFeedbackAsync(Feedback feedback)
    {
        if (feedback.NivelSatisfacao <= 6)
        {
            var mensagem = await _iaService.GerarMensagemAsync(
                feedback.NivelSatisfacao
            );

            feedback.MensagemGeradaIA = mensagem;
        }
    }
}