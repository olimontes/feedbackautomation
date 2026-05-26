using ClosedXML.Excel;
using FeedbackAutomation.Data;
using FeedbackAutomation.Entities;

namespace FeedbackAutomation.Imports;

public class ExcelImporter
{
    private readonly AppDbContext _context;

    public ExcelImporter(AppDbContext context)
    {
        _context = context;
    }

    public void Importar(string caminhoArquivo)
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

                // Verifica se o cliente já existe
                var cliente = _context.Clientes
                    .FirstOrDefault(c => c.Cnpj == cnpj);

                // Se não existir, cria
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

                    // Salva para gerar o ID
                    _context.SaveChanges();
                }

                var feedback = new Feedback
                {
                    ClienteId = cliente.Id,
                    EquipeDescricao = row.Cell(6).GetValue<string>(),
                    NivelSatisfacao = row.Cell(7).GetValue<int>(),
                    DataVerificacaoQualidade = DateTime.SpecifyKind(
                        DateTime.Parse(row.Cell(8).GetValue<string>()),
                        DateTimeKind.Utc
                    )
                };

                _context.Feedbacks.Add(feedback);

                _context.SaveChanges();

                Console.WriteLine($"Linha {linha} importada com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na linha {linha}: {ex.Message}");
            }
        }

        Console.WriteLine("Importação finalizada.");
    }
}









