using InvestScoreBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestScoreBackend.Infrastructure.Persistence
{
    public class InvestScoreDbContext : DbContext
    {
        public InvestScoreDbContext(DbContextOptions<InvestScoreDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Asset> Assets { get; set; }
        public DbSet<FileRecord> FileRecords { get; set; }
        public DbSet<AssetHead> AssetHeads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // AssetHead
            modelBuilder.Entity<AssetHead>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ProcessedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.MachineIp).HasMaxLength(50);

                // Relacionamento com FileRecord (um FileRecord pode ter vários AssetHeads)
                entity.HasOne<FileRecord>()
                      .WithMany()
                      .HasForeignKey(e => e.FileRecordId)
                      .OnDelete(DeleteBehavior.Restrict); // evita cascata múltipla
            });

            // Asset
            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Ticker).IsRequired().HasMaxLength(10);
                entity.Property(e => e.ROE).HasPrecision(18, 2);
                entity.Property(e => e.DY).HasPrecision(18, 2);
                entity.Property(e => e.CAGR).HasPrecision(18, 2);
                entity.Property(e => e.Liquidez).HasPrecision(18, 2);
                entity.Property(e => e.Score).HasPrecision(18, 2);

                // Relacionamento com AssetHead (um AssetHead pode ter vários Assets)
                entity.HasOne<AssetHead>(a => a.AssetHead)
                      .WithMany(h => h.Assets)
                      .HasForeignKey(a => a.AssetHeadId)
                      .OnDelete(DeleteBehavior.Restrict); // evita cascata múltipla
            });

            // FileRecord
            modelBuilder.Entity<FileRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.IsAvailable).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }


    }
}
