using Agenda.Api.Dtos.Contacts;
using Agenda.Api.Services.Contacts;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _service;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(IContactService service, ILogger<ContactsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactResponseDto>>> GetAll()
        {
            var contacts = await _service.GetAllAsync();
            return Ok(contacts);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ContactResponseDto>> GetById(Guid id)
        {
            var contact = await _service.GetByIdAsync(id);
            if (contact == null) return NotFound();

            return Ok(contact);
        }

        [HttpPost]
        public async Task<ActionResult<ContactResponseDto>> Create([FromBody] CreateContactDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro de regra de negócio ao criar contato");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ContactResponseDto>> Update(Guid id, [FromBody] UpdateContactDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null) return NotFound();

                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro de regra de negócio ao atualizar contato");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
