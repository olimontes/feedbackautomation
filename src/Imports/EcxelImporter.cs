using ClosedXML.Excel;
using FeedbackAutomation.AI;
using FeedbackAutomation.Data;
using FeedbackAutomation.Entities;
using FeedbackAutomation.Utils;

namespace FeedbackAutomation.Imports;

public class ExcelImporter
{
    private readonly AppDbContext _context;
    private readonly IAService _iaService;
    private readonly EvolutionService _evolutionService;

    public ExcelImporter(
        AppDbContext context,
        IAService iaService,
        EvolutionService evolutionService
    )
    {
        _context = context;
        _iaService = iaService;
        _evolutionService = evolutionService;
    }

    public async Task ImportarAsync(string caminhoArquivo)
    {
        using var workbook = new XLWorkbook(caminhoArquivo);

        var worksheet = workbook.Worksheet(1);

        var rows = worksheet.RowsUsed().Skip(1);

        int linha = 1;

        foreach (var row in rows)
        {
            linha++;

            try
            {
                var cnpj = row.Cell(3).GetValue<string>();
                var telefoneNormalizado =
                    TelefoneHelper.NormalizarTelefoneWhatsApp(
                        row.Cell(5).GetValue<string>()
                    );

                // Ignora linhas vazias
                if (string.IsNullOrWhiteSpace(cnpj))
                {
                    Console.WriteLine($"Linha {linha} ignorada.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(telefoneNormalizado))
                {
                    Console.WriteLine(
                        $"Linha {linha}: cliente sem telefone cadastrado."
                    );
                }
                else if (!TelefoneHelper.TelefoneValidoParaWhatsApp(
                    telefoneNormalizado
                ))
                {
                    Console.WriteLine(
                        $"Linha {linha}: telefone invalido para WhatsApp: {telefoneNormalizado}."
                    );
                }

                // Busca cliente existente
                var cliente = _context.Clientes
                    .FirstOrDefault(c => c.Cnpj == cnpj);

                // Cria cliente se não existir
                if (cliente == null)
                {
                    cliente = new Cliente
                    {
                        CodigoCliente = row.Cell(1).GetValue<string>(),
                        NomeRede = row.Cell(2).GetValue<string>(),
                        Cnpj = cnpj,
                        Responsavel = row.Cell(4).GetValue<string>(),
                        Telefone = telefoneNormalizado
                    };

                    _context.Clientes.Add(cliente);

                    await _context.SaveChangesAsync();
                }
                else if (
                    TelefoneHelper.TelefoneValidoParaWhatsApp(
                        telefoneNormalizado
                    )
                    && cliente.Telefone != telefoneNormalizado
                )
                {
                    cliente.Telefone = telefoneNormalizado;
                }

                var nivelSatisfacao = row.Cell(7).GetValue<int>();
                var contextoFeedback = row.Cell(6).GetValue<string>();

                // Trata data
                var dataTexto = row.Cell(8).GetValue<string>();

                var data = DateTime.Parse(dataTexto);

                var dataUtc = DateTime.SpecifyKind(
                    data,
                    DateTimeKind.Utc
                );

                // Geração IA
                string? mensagemIA = null;

                if (nivelSatisfacao <= 6)
                {
                    mensagemIA = await _iaService.GerarMensagemAsync(
                        cliente.Responsavel,
                        nivelSatisfacao,
                        contextoFeedback
                    );
                }

                var feedback = new Feedback
                {
                    ClienteId = cliente.Id,
                    EquipeDescricao = contextoFeedback,
                    NivelSatisfacao = nivelSatisfacao,
                    DataVerificacaoQualidade = dataUtc,
                    MensagemGeradaIA = mensagemIA
                };

                _context.Feedbacks.Add(feedback);

                await _context.SaveChangesAsync();

                await EnviarMensagemWhatsAppAsync(
                    cliente,
                    mensagemIA,
                    linha
                );

                Console.WriteLine(
                    $"Linha {linha} importada com sucesso."
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Erro na linha {linha}: {ex.Message}"
                );
            }
        }

        Console.WriteLine("Importação finalizada.");
    }

    private async Task EnviarMensagemWhatsAppAsync(
        Cliente cliente,
        string? mensagem,
        int linha
    )
    {
        if (string.IsNullOrWhiteSpace(mensagem))
        {
            return;
        }

        if (!TelefoneHelper.TelefoneValidoParaWhatsApp(cliente.Telefone))
        {
            Console.WriteLine(
                $"Linha {linha}: mensagem nao enviada. Cliente sem telefone valido."
            );
            return;
        }

        try
        {
            await _evolutionService.SendMessage(
                cliente.Telefone,
                mensagem
            );

            Console.WriteLine(
                $"Linha {linha}: mensagem enviada para {cliente.Telefone}."
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Linha {linha}: feedback salvo, mas houve erro ao enviar WhatsApp: {ex.Message}"
            );
        }
    }
}
