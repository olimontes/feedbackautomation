namespace FeedbackAutomation.Utils;

public static class TelefoneHelper
{
    public static string LimparTelefone(string? telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
        {
            return string.Empty;
        }

        return new string(
            telefone.Where(char.IsDigit).ToArray()
        );
    }

    public static string NormalizarTelefoneWhatsApp(string? telefone)
    {
        var telefoneLimpo = LimparTelefone(telefone);

        if (telefoneLimpo.Length == 11)
        {
            return $"55{telefoneLimpo}";
        }

        return telefoneLimpo;
    }

    public static bool TelefoneValidoParaWhatsApp(string? telefone)
    {
        var telefoneNormalizado = NormalizarTelefoneWhatsApp(telefone);

        return telefoneNormalizado.Length == 13
            && telefoneNormalizado.StartsWith("55");
    }
}
