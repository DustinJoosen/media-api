using Media.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Media.Persistence
{
    public class MediaDbContext : DbContext
    {
        public DbSet<AuthToken> AuthTokens { get; set; }
        public DbSet<MediaItem> MediaItems { get; set; }

        public MediaDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthToken>()
                .HasIndex(token => token.Name)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            // Update all entities their CreatedOn and UpdatedOn fields.
            foreach (var entity in this.ChangeTracker.Entries())
            {
                if (entity.Entity is not AuditableEntity auditableEntity)
                    continue;

                if (entity.State == EntityState.Added)
                    auditableEntity.CreatedOn = DateTime.UtcNow;

                if (entity.State == EntityState.Modified)
                    auditableEntity.UpdatedOn = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Update all entities their CreatedOn and UpdatedOn fields.
            foreach (var entity in this.ChangeTracker.Entries())
            {
                if (entity.Entity is not AuditableEntity auditableEntity)
                    continue;

                if (entity.State == EntityState.Added)
                    auditableEntity.CreatedOn = DateTime.UtcNow;

                if (entity.State == EntityState.Modified)
                    auditableEntity.UpdatedOn = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
