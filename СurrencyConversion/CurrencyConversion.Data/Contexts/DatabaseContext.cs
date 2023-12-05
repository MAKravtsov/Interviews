using CurrencyConversion.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyConversion.Data.Contexts;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
        
    }
    
    public virtual DbSet<UserEntity> Users { get; set; }
    
    public virtual DbSet<CurrencyEntity> Currencies { get; set; }
    
    public virtual DbSet<AccountEntity> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountEntity>()
            .HasAlternateKey(nameof(AccountEntity.UserId), nameof(AccountEntity.CurrencyId));

        base.OnModelCreating(modelBuilder);
    }
}