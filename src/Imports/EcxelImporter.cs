using ClosedXML.Excel;
using FeedbackAutomation.AI;
using FeedbackAutomation.Data;
using FeedbackAutomation.Entities;

namespace FeedbackAutomation.Imports;

public class ExcelImporter
{
    private readonly AppDbContext _context;
    private readonly IAService _iaService;

    public ExcelImporter(
        AppDbContext context,
        IAService iaService
    )
    {
        _context = context;
        _iaService = iaService;
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

                // Ignora linhas vazias
                if (string.IsNullOrWhiteSpace(cnpj))
                {
                    Console.WriteLine($"Linha {linha} ignorada.");
                    continue;
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
                        Telefone = row.Cell(5).GetValue<string>()
                    };

                    _context.Clientes.Add(cliente);

                    await _context.SaveChangesAsync();
                }

                var nivelSatisfacao = row.Cell(7).GetValue<int>();

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
                        nivelSatisfacao
                    );
                }

                var feedback = new Feedback
                {
                    ClienteId = cliente.Id,
                    EquipeDescricao = row.Cell(6).GetValue<string>(),
                    NivelSatisfacao = nivelSatisfacao,
                    DataVerificacaoQualidade = dataUtc,
                    MensagemGeradaIA = mensagemIA
                };

                _context.Feedbacks.Add(feedback);

                await _context.SaveChangesAsync();

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
}