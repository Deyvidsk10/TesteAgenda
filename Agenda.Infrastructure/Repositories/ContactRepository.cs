using Agenda.Domain.Entities;
using Agenda.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Infrastructure.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly AgendaDbContext _context;

        public ContactRepository(AgendaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Contact>> GetAllAsync()
        {
            return await _context.Contacts.AsNoTracking().ToListAsync();
        }

        public async Task<Contact?> GetByIdAsync(Guid id)
        {
            return await _context.Contacts.FindAsync(id);
        }

        public async Task AddAsync(Contact contact)
        {
            await _context.Contacts.AddAsync(contact);
        }

        public async Task UpdateAsync(Contact contact)
        {
            _context.Contacts.Update(contact);
        }

        public async Task DeleteAsync(Contact contact)
        {
            _context.Contacts.Remove(contact);
        }

        public async Task<bool> EmailExistsAsync(string email, Guid? ignoreId = null)
        {
            var query = _context.Contacts.AsQueryable()
                .Where(c => c.Email == email);

            if (ignoreId.HasValue)
            {
                query = query.Where(c => c.Id != ignoreId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
