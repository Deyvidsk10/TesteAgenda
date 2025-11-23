namespace Agenda.Api.Dtos.Contacts
{
    public class UpdateContactDto
    {

        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
    }
}
