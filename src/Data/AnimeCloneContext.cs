using AnimeClone.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimeClone.Data;

public class AnimeCloneContext : DbContext
{
    public DbSet<Anime> Animes { get; set; }

    public DbSet<AnimeInsert> AnimeInsert { get; set; }

    public DbSet<AnimeTag> AnimeTag { get; set; }
    public DbSet<AnimeSeason> AnimeSeasons { get; set; }
    public DbSet<Duration> Durations { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public AnimeCloneContext(DbContextOptions<AnimeCloneContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSeeding((context, seed) => { seed = true; });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Anime - AnimeSeason (one-to-one relationship)
        modelBuilder.Entity<Anime>()
            .HasOne(a => a.AnimeSeason); // One Anime can have one AnimeSeasons

        // Anime - Duration (one-to-one relationship)
        modelBuilder.Entity<Anime>()
            .HasOne(a => a.Duration);

        modelBuilder.Entity<AnimeInsert>();

        // Ensure unique AnimeSeason combination
        modelBuilder.Entity<AnimeSeason>()
            .HasIndex(a => new { a.Season, a.Year })
            .IsUnique();

        // Ensure unique Duration combination
        modelBuilder.Entity<Duration>()
            .HasIndex(d => new { d.Value, d.Unit })
            .IsUnique();
        // Ensure unique AnimeSeason combination
        modelBuilder.Entity<Tag>()
            .HasIndex(t => new { t.Name })
            .IsUnique();

        // Configure many-to-many relationship between Anime and Tag
        // modelBuilder.Entity<AnimeTag>()
        //     .HasKey(at => new { at.AnimeId, at.TagId });

        modelBuilder.Entity<AnimeTag>()
            .HasOne(at => at.Anime)
            .WithMany(a => a.AnimeTags)
            .HasForeignKey(at => at.AnimeId);

        modelBuilder.Entity<AnimeTag>()
            .HasOne(at => at.Tag)
            .WithMany(t => t.AnimeTags)
            .HasForeignKey(at => at.TagId);
    }
}