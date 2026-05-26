using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackAutomation.Entities;

[Table("Feedbacks")]
public class Feedback
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public Cliente Cliente { get; set; } = null!;

    public string EquipeDescricao { get; set; } = string.Empty;

    public int NivelSatisfacao { get; set; }

    public DateTime DataVerificacaoQualidade { get; set; }

    public string? MensagemGeradaIA { get; set; }
}