using Microsoft.EntityFrameworkCore;
using ContactManagement.Domain.Entities;

namespace ContactManagement.Infrastructure.Data
{
    public class ContactDbContext : DbContext
    {
        public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options) { }

        public DbSet<Contact> Contacts { get; set; }
    }
}
