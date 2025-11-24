using Agenda.Api.Dtos.Contacts;
using Agenda.Api.Models;
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
            try
            {
                var contacts = await _service.GetAllAsync();
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao listar contatos");

                return StatusCode(500, new ErrorResponse
                {
                    Message = "Erro inesperado ao listar contatos."
                });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ContactResponseDto>> GetById(Guid id)
        {
            try
            {
                var contact = await _service.GetByIdAsync(id);

                if (contact == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Contato não encontrado."
                    });
                }

                return Ok(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar contato por ID");

                return StatusCode(500, new ErrorResponse
                {
                    Message = "Erro inesperado ao buscar contato."
                });
            }
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

                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar contato");

                return StatusCode(500, new ErrorResponse
                {
                    Message = "Erro inesperado ao criar contato."
                });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ContactResponseDto>> Update(Guid id, [FromBody] UpdateContactDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);

                if (updated == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Contato não encontrado."
                    });
                }

                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro de regra de negócio ao atualizar contato");

                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar contato");

                return StatusCode(500, new ErrorResponse
                {
                    Message = "Erro inesperado ao atualizar contato."
                });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Contato não encontrado."
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao excluir contato");

                return StatusCode(500, new ErrorResponse
                {
                    Message = "Erro inesperado ao excluir contato."
                });
            }
        }
    }
}
