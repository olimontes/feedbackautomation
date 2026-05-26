using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackAutomation.Entities;

[Table("Clientes")]
public class Cliente
{
    public int Id { get; set; }

    public string CodigoCliente { get; set; } = string.Empty;

    public string NomeRede { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Responsavel { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;
}