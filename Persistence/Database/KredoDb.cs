using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class KredoDb : DbContext
{
    public KredoDb(DbContextOptions<KredoDb> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<LoanApplication> Applications => Set<LoanApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KredoDb).Assembly);
    }
}
