using Games.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Games.Infrastructure.Data;

public class DbGames : DbContext
{
    public DbGames(DbContextOptions<DbGames> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GamesEnt>().ToTable("JOGOS");
        modelBuilder.Entity<PromotionEnt>().ToTable("PROMOCAO");
        modelBuilder.Entity<LibraryEnt>().ToTable("BIBLIOTECA");
        modelBuilder.Entity<ILoggerEnt>().ToTable("LOGS");

        modelBuilder.Entity<GamesEnt>()
            .Property(j => j.Preco)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PromotionEnt>()
            .Property(j => j.Valor)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PromotionEnt>()
            .Property(p => p.IdJogos)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null));

    }

    public DbSet<GamesEnt> JOGOS { get; set; }
    public DbSet<PromotionEnt> PROMOCAO { get; set; }
    public DbSet<LibraryEnt> BIBLIOTECA_JOGOS { get; set; }
    public DbSet<ILoggerEnt> LOGS { get; set; }

}
