using Microsoft.EntityFrameworkCore;
using TrainingManagement.Domain.Entities;
using System;

namespace TrainingManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Training> Trainings { get; set; }
    public DbSet<UserTraining> UserTrainings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
        });

        // Training configuration
        modelBuilder.Entity<Training>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TrainingName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TrainingUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Platform).IsRequired().HasMaxLength(50);
        });

        // UserTraining configuration
        modelBuilder.Entity<UserTraining>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.TrainingId }).IsUnique();

            entity.Property(e => e.Comments).HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserTrainings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Training)
                .WithMany(t => t.UserTrainings)
                .HasForeignKey(e => e.TrainingId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}