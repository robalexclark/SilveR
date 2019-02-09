using Microsoft.EntityFrameworkCore;

namespace SilveR.Models
{
    public partial class SilveRContext : DbContext
    {
        public SilveRContext(DbContextOptions<SilveRContext> options)
            : base(options)
        {   
        }

        public virtual DbSet<Analysis> Analyses { get; set; }
        public virtual DbSet<Argument> Arguments { get; set; }
        public virtual DbSet<Dataset> Datasets { get; set; }
        public virtual DbSet<Script> Scripts { get; set; }
        public virtual DbSet<UserOption> UserOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // I had removed this

            modelBuilder.Entity<Analysis>(entity =>
            {
                entity.HasOne(d => d.Dataset)
                    .WithMany(p => p.Analysis)
                    .HasForeignKey(d => d.DatasetID)
                    .HasConstraintName("FK_Analyses_Datasets");

                entity.HasOne(d => d.Script)
                    .WithMany(p => p.Analysis)
                    .HasForeignKey(d => d.ScriptID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Analyses_Scripts");

                entity.Property(e => e.Tag).IsUnicode(false);
            });

            modelBuilder.Entity<Argument>(entity =>
            {
                entity.HasOne(d => d.Analysis)
                    .WithMany(p => p.Arguments)
                    .HasForeignKey(d => d.AnalysisID)
                    .HasConstraintName("FK_Arguments_Analyses");
            });

            modelBuilder.Entity<Dataset>(entity =>
            {
                entity.Property(e => e.DatasetName);
                entity.Property(e => e.TheData);
            });

            modelBuilder.Entity<Script>(entity =>
            {
                entity.Property(e => e.ScriptDisplayName);
                entity.Property(e => e.ScriptFileName);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}