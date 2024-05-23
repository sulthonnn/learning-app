using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServiceEsgDataHub.Services;
using ServiceLearningApp.Model;

namespace ServiceLearningApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly UserResolverService userResolverService;
        public ApplicationDbContext(DbContextOptions options, UserResolverService userResolverService) : base(options)
        {
            this.userResolverService = userResolverService;
        }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<SubChapter> SubChapters { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseTransaction> ExerciseTransactions { get; set; }
        public DbSet<Upload> Uploads { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AddAudit();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        
        private void AddAudit()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is IAuditableEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((IAuditableEntity)entity.Entity).CreatedAt = DateTime.UtcNow;
                    ((IAuditableEntity)entity.Entity).CreatedBy = userResolverService.GetNameIdentifier();
                }
                else
                {
                    var createdAtFound = false;
                    var createdByFound = false;

                    foreach (var prop in entity.Properties)
                    {
                        if (createdAtFound && createdByFound)
                            break;

                        if (prop.Metadata.Name == "CreatedAt")
                        {
                            prop.IsModified = false;
                            createdAtFound = true;
                        }

                        if (prop.Metadata.Name == "CreatedBy")
                        {
                            prop.IsModified = false;
                            createdByFound = true;
                        }
                    }

                    ((IAuditableEntity)entity.Entity).UpdatedAt = DateTime.UtcNow;
                    ((IAuditableEntity)entity.Entity).UpdatedBy = userResolverService.GetNameIdentifier();
                }
            }
        }

    }
}
