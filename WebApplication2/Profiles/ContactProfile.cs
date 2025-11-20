using Agenda.Api.Dtos.Contacts;
using Agenda.Domain.Entities;
using AutoMapper;

namespace Agenda.Api.Profiles
{
    public class ContactProfile : Profile
    {
        public ContactProfile()
        {
            
            CreateMap<CreateContactDto, Contact>();

      
            CreateMap<UpdateContactDto, Contact>();

            
            CreateMap<Contact, ContactResponseDto>();
        }
    }
}

