namespace Agenda.Api.Validators.Contacts
{
    public class UpdateContactDtoValidator
    {

        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
