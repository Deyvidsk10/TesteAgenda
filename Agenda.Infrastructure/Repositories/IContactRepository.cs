using Agenda.Domain.Entities;

namespace Agenda.Infrastructure.Repositories
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllAsync();
        Task<Contact?> GetByIdAsync(Guid id);
        Task AddAsync(Contact contact);
        Task UpdateAsync(Contact contact);
        Task DeleteAsync(Contact contact);
        Task<bool> EmailExistsAsync(string email, Guid? ignoreId = null);
        Task<int> SaveChangesAsync();
    }
}
