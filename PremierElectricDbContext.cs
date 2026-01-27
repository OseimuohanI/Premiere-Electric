// PremierElectricDbContext.cs - Entity Framework DbContext
using Microsoft.EntityFrameworkCore;
using PremierElectric.Domain.Entities;

namespace PremierElectric.Infrastructure.Data
{
    public class PremierElectricDbContext : DbContext
    {
        public PremierElectricDbContext(DbContextOptions<PremierElectricDbContext> options)
            : base(options)
        {
        }

        public DbSet<ContactSubmission> ContactSubmissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ContactSubmission configuration
            modelBuilder.Entity<ContactSubmission>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(e => e.ServiceCategory)
                    .HasMaxLength(50);

                entity.Property(e => e.PreferredContact)
                    .HasMaxLength(20);

                entity.Property(e => e.AdminNotes)
                    .HasMaxLength(5000);

                entity.Property(e => e.Status)
                    .HasConversion<int>();

                entity.Property(e => e.SubmittedAt)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                // Create indices for better query performance
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.SubmittedAt);
            });
        }
    }
}
