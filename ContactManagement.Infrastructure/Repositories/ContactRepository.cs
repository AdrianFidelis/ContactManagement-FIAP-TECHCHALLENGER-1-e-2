using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Repositories;
using ContactManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactManagement.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly ContactDbContext _context;

    public ContactRepository(ContactDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Contact>> GetAllAsync()
    {
        return await _context.Contacts.ToListAsync();
    }

    public async Task<Contact?> GetByIdAsync(int id)
    {
        return await _context.Contacts.FindAsync(id);
    }

    public async Task AddAsync(Contact contact)
    {
        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Contact contact)
    {
        _context.Contacts.Update(contact);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact != null)
        {
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Contact>> GetAllByDddAsync(int ddd)
    {
        return await _context.Contacts
            .Where(c => c.Phone.RegionalCode == ddd) // Filtrar contatos pelo DDD
            .ToListAsync();
    }
}
