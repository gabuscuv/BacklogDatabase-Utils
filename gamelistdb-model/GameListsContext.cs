using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GameListDB.Model;

public partial class GameListsContext : DbContext
{
    public GameListsContext()
    {
    }

    public GameListsContext(DbContextOptions<GameListsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Backlog> Backlogs { get; set; }

    public virtual DbSet<GamesId> GamesIds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Name=ConnectionStrings.GameList");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Backlog>(entity =>
        {
            entity.ToTable("Backlog");

            entity.HasIndex(e => e.Id, "IX_Backlog_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Beaten)
                .HasDefaultValueSql("0")
                .HasColumnName("beaten");
            entity.Property(e => e.Completed)
                .HasDefaultValueSql("0")
                .HasColumnName("completed");
            entity.Property(e => e.Completedyear).HasColumnName("completedyear");
            entity.Property(e => e.CurrentTime).HasColumnName("current_time");
            entity.Property(e => e.Dependence).HasColumnName("dependence");
            entity.Property(e => e.GameSeriesId).HasColumnName("gameSeriesID");
            entity.Property(e => e.InfiniteGame).HasDefaultValueSql("0");
            entity.Property(e => e.MaxTime).HasColumnName("max_time");
            entity.Property(e => e.MinTime).HasColumnName("min_time");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Nsfw)
                .HasDefaultValueSql("0")
                .HasColumnName("nsfw");
            entity.Property(e => e.Plataform).HasColumnName("plataform");
            entity.Property(e => e.Playsite).HasColumnName("playsite");
            entity.Property(e => e.Priority)
                .HasDefaultValueSql("5")
                .HasColumnName("priority");
            entity.Property(e => e.Releaseyear).HasColumnName("releaseyear");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'Not Started'")
                .HasColumnName("status");
            entity.Property(e => e.Type).HasDefaultValueSql("'Game'");
            entity.Property(e => e.WhenStart).HasColumnName("when_start");
        });

        modelBuilder.Entity<GamesId>(entity =>
        {
            entity.ToTable("GamesID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IgdbId).HasColumnName("igdbID");
            entity.Property(e => e.PsnProfile).HasColumnName("psnProfile");
            entity.Property(e => e.PsstoreId).HasColumnName("PSStoreID");
            entity.Property(e => e.SteamId).HasColumnName("SteamID");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.InverseIdNavigation)
                .HasForeignKey<GamesId>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
