using ContactManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactManagement.Infrastructure.Data
{
    public class ContactDbContext : DbContext
    {
        public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options) { }

        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>()
                .OwnsOne(c => c.Phone) // Define que `Phone` é um tipo de valor embutido em `Contact`
                .Property(p => p.RegionalCode)
                .IsRequired()
                .HasColumnName("RegionalCode");

            modelBuilder.Entity<Contact>()
                .OwnsOne(c => c.Phone)
                .Property(p => p.CountryCode)
                .IsRequired()
                .HasColumnName("CountryCode");

            modelBuilder.Entity<Contact>()
                .OwnsOne(c => c.Phone)
                .Property(p => p.NumberPhone)
                .IsRequired()
                .HasColumnName("NumberPhone");
        }
    }
}
