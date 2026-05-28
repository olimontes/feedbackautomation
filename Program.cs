using FeedbackAutomation.Data;
using FeedbackAutomation.Imports;
using Microsoft.EntityFrameworkCore;
using FeedbackAutomation.AI;
using FeedbackAutomation.Services;

var builder = WebApplication.CreateBuilder(args);

// Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Services
builder.Services.AddControllers();

builder.Services.AddScoped<ExcelImporter>();
builder.Services.AddScoped<IAService>();

builder.Services.AddHttpClient<EvolutionService>();

var app = builder.Build();

app.MapControllers();

// Importação automática do Excel
using (var scope = app.Services.CreateScope())
{
    var importer =
        scope.ServiceProvider.GetRequiredService<ExcelImporter>();

    var excelPath = "nps04.xlsx";

    if (File.Exists(excelPath))
    {
        await importer.ImportarAsync(excelPath);
    }
    else
    {
        app.Logger.LogWarning(
            "Arquivo de importacao nao encontrado: {ExcelPath}",
            excelPath
        );
    }
}

app.Run();