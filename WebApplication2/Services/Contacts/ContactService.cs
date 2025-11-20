using Agenda.Api.Dtos.Contacts;
using Agenda.Domain.Entities;
using Agenda.Infrastructure.Repositories;
using AutoMapper;

namespace Agenda.Api.Services.Contacts
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _repository;
        private readonly IMapper _mapper;

        public ContactService(IContactRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ContactResponseDto>> GetAllAsync()
        {
            var contacts = await _repository.GetAllAsync();
            return _mapper.Map<List<ContactResponseDto>>(contacts);
        }

        public async Task<ContactResponseDto?> GetByIdAsync(Guid id)
        {
            var contact = await _repository.GetByIdAsync(id);
            if (contact == null) return null;

            return _mapper.Map<ContactResponseDto>(contact);
        }

        public async Task<ContactResponseDto> CreateAsync(CreateContactDto dto)
        {
            var emailExists = await _repository.EmailExistsAsync(dto.Email);
            if (emailExists)
                throw new InvalidOperationException("Já existe um contato com esse e-mail.");

            var contact = _mapper.Map<Contact>(dto);

            await _repository.AddAsync(contact);
            await _repository.SaveChangesAsync();

            return _mapper.Map<ContactResponseDto>(contact);
        }

        public async Task<ContactResponseDto?> UpdateAsync(Guid id, UpdateContactDto dto)
        {
            var contact = await _repository.GetByIdAsync(id);
            if (contact == null) return null;

            var emailExists = await _repository.EmailExistsAsync(dto.Email, ignoreId: id);
            if (emailExists)
                throw new InvalidOperationException("Já existe um contato com esse e-mail.");

            _mapper.Map(dto, contact);

            await _repository.UpdateAsync(contact);
            await _repository.SaveChangesAsync();

            return _mapper.Map<ContactResponseDto>(contact);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var contact = await _repository.GetByIdAsync(id);
            if (contact == null) return false;

            await _repository.DeleteAsync(contact);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
