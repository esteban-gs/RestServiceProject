using Microsoft.EntityFrameworkCore;
using RestServiceProject.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestServiceProject.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base (options) { }

        public DbSet<User> Users { get; set; }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is ITrackable trackable && entry.State == EntityState.Added)
                {
                    trackable.CreatedDate = DateTime.Now;
                }
            }
        }
    }
}
