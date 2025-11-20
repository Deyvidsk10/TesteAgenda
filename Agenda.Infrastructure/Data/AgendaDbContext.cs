using Agenda.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Infrastructure.Data
{
    public class AgendaDbContext : DbContext
    {
        public AgendaDbContext(DbContextOptions<AgendaDbContext> options)
           : base(options)
        {
        }

        public DbSet<Contact> Contacts => Set<Contact>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(c => c.Phone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(c => c.Email)
                    .IsUnique();
            });
        }
    }
}
