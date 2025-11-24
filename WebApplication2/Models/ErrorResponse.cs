namespace Agenda.Api.Models
{
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<string>? Details { get; set; }
    }
}
