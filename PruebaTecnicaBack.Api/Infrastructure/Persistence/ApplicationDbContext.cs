using Microsoft.EntityFrameworkCore;
using PruebaTecnicaBack.Domain.Entities;

namespace PruebaTecnicaBack.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Currency> Currencies => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.Property(u => u.Name).HasMaxLength(100);
            b.Property(u => u.Email).HasMaxLength(254).UseCollation("NOCASE");
            b.Property(u => u.PasswordHash).HasMaxLength(128).IsRequired(false);
            
            b.HasIndex(u => u.Email).IsUnique();

            b.HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Address>(b =>
        {
            b.Property(a => a.Street).HasMaxLength(200);
            b.Property(a => a.City).HasMaxLength(100);
            b.Property(a => a.Country).HasMaxLength(100);
            b.Property(a => a.ZipCode).HasMaxLength(20);
        });

        modelBuilder.Entity<Currency>(b =>
        {
            b.Property(c => c.Name).HasMaxLength(100);
            
            b.Property(c => c.Code)
                .HasMaxLength(3)
                .IsFixedLength()
                .UseCollation("NOCASE");
                
            b.HasIndex(c => c.Code).IsUnique();

            b.Property(c => c.RateToBase).HasPrecision(18, 4); 
            b.ToTable(table => table.HasCheckConstraint(
                name: "CK_Currency_RatePositive", 
                sql: "\"RateToBase\" > 0"
            ));
        });
    }
}