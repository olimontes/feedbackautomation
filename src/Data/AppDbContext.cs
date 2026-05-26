using FeedbackAutomation.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeedbackAutomation.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; } = null!;

    public DbSet<Feedback> Feedbacks { get; set; } = null!;
}
