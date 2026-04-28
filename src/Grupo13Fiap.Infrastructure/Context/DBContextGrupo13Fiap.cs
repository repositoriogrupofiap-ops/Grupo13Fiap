using Grupo13Fiap.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Grupo13Fiap.Infrastructure.Context;

public class DBContextGrupo13Fiap(DbContextOptions<DBContextGrupo13Fiap> options) : DbContext(options)
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Library> Libraries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Category).HasConversion<int>();
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasMany(e => e.Games)
                  .WithMany()
                  .UsingEntity(j => j.ToTable("StoreGames"));
        });

        modelBuilder.Entity<Library>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasMany(e => e.Games)
                  .WithMany()
                  .UsingEntity(j => j.ToTable("LibraryGames"));
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.IdentityUserId).IsRequired().HasMaxLength(450);
            entity.Ignore(e => e.Roles);
            entity.HasOne(e => e.Library)
                  .WithOne()
                  .HasForeignKey<User>(e => e.LibraryId);
        });
    }
}
