using FeedbackAutomation.Data;
using FeedbackAutomation.Imports;
using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var importer = scope.ServiceProvider.GetRequiredService<ExcelImporter>();
    var excelPath = "nps04.xlsx";

    if (File.Exists(excelPath))
    {
        importer.Importar(excelPath);
    }
    else
    {
        app.Logger.LogWarning("Arquivo de importacao nao encontrado: {ExcelPath}", excelPath);
    }
}

app.Run();
