using ContactManagement.Domain.Entities;
using ContactManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactManagement.Infrastructure.Repositories;

public class ContactRepository
{
    private readonly ContactDbContext _context;

    public ContactRepository(ContactDbContext context)
    {
        _context = context;
    }

    public async Task<List<Contact>> GetAllAsync() => await _context.Contacts.ToListAsync();

    public async Task<Contact?> GetByIdAsync(int id) => await _context.Contacts.FindAsync(id);

    public async Task AddAsync(Contact contact)
    {
        await _context.Contacts.AddAsync(contact);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Contact contact)
    {
        _context.Contacts.Update(contact);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var contact = await GetByIdAsync(id);
        if (contact != null)
        {
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
        }
    }
}
