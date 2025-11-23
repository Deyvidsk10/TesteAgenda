using Agenda.Api.Dtos.Contacts;

namespace Agenda.Api.Services.Contacts
{
    public interface IContactService
    {
        Task<List<ContactResponseDto>> GetAllAsync();
        Task<ContactResponseDto?> GetByIdAsync(Guid id);
        Task<ContactResponseDto> CreateAsync(CreateContactDto dto);
        Task<ContactResponseDto?> UpdateAsync(Guid id, UpdateContactDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
