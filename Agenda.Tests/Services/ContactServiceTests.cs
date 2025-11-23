using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Agenda.Api.Dtos.Contacts;
using Agenda.Api.Services.Contacts;
using Agenda.Domain.Entities;
using Agenda.Infrastructure.Repositories;
using AutoMapper;
using Moq;
using Xunit;

namespace Agenda.Tests.Services
{
    public class ContactServiceTests
    {
        private readonly Mock<IContactRepository> _contactRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ContactService _service;

        public ContactServiceTests()
        {
            _contactRepositoryMock = new Mock<IContactRepository>();
            _mapperMock = new Mock<IMapper>();

            _service = new ContactService(_contactRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_DeveCriarContato_QuandoEmailNaoExiste()
        {

            var createDto = new CreateContactDto
            {
                Name = "Teste",
                Email = "teste@teste.com",
                Phone = "81999999999"
            };

            _contactRepositoryMock
                .Setup(r => r.EmailExistsAsync(createDto.Email, null))
                .ReturnsAsync(false);

            var contactEntity = new Contact
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Email = createDto.Email,
                Phone = createDto.Phone
            };


            _mapperMock
                .Setup(m => m.Map<Contact>(createDto))
                .Returns(contactEntity);

            var responseDto = new ContactResponseDto
            {
                Id = contactEntity.Id,
                Name = contactEntity.Name,
                Email = contactEntity.Email,
                Phone = contactEntity.Phone
            };


            _mapperMock
                .Setup(m => m.Map<ContactResponseDto>(contactEntity))
                .Returns(responseDto);


            var result = await _service.CreateAsync(createDto);


            Assert.Equal(createDto.Name, result.Name);
            Assert.Equal(createDto.Email, result.Email);
            Assert.Equal(createDto.Phone, result.Phone);


            _contactRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Contact>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarExcecao_QuandoEmailJaExiste()
        {

            var createDto = new CreateContactDto
            {
                Name = "Teste 2",
                Email = "jaexiste@teste.com",
                Phone = "81988888888"
            };

            _contactRepositoryMock
                .Setup(r => r.EmailExistsAsync(createDto.Email, null))
                .ReturnsAsync(true);


            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateAsync(createDto));

            Assert.Equal("Já existe um contato com este e-mail.", ex.Message);


            _contactRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaDeContatos()
        {

            var contacts = new List<Contact>
            {
                new Contact { Id = Guid.NewGuid(), Name = "A", Email = "a@a.com", Phone = "1111" },
                new Contact { Id = Guid.NewGuid(), Name = "B", Email = "b@b.com", Phone = "2222" }
            };

            _contactRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(contacts);

            var responseList = new List<ContactResponseDto>
            {
                new ContactResponseDto { Id = contacts[0].Id, Name = contacts[0].Name, Email = contacts[0].Email, Phone = contacts[0].Phone },
                new ContactResponseDto { Id = contacts[1].Id, Name = contacts[1].Name, Email = contacts[1].Email, Phone = contacts[1].Phone }
            };

            _mapperMock
                .Setup(m => m.Map<List<ContactResponseDto>>(contacts))
                .Returns(responseList);


            var result = await _service.GetAllAsync();


            Assert.Equal(2, result.Count);
            Assert.Equal("A", result[0].Name);
            Assert.Equal("B", result[1].Name);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarNull_QuandoContatoNaoEncontrado()
        {

            var id = Guid.NewGuid();
            var dto = new UpdateContactDto
            {
                Name = "Novo",
                Email = "novo@teste.com",
                Phone = "0000"
            };

            _contactRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Contact?)null);


            var result = await _service.UpdateAsync(id, dto);


            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_DeveLancarExcecao_QuandoEmailJaExisteEmOutroContato()
        {

            var id = Guid.NewGuid();

            var existing = new Contact
            {
                Id = id,
                Name = "Antigo",
                Email = "antigo@teste.com",
                Phone = "1111"
            };

            var dto = new UpdateContactDto
            {
                Name = "Novo",
                Email = "duplicado@teste.com",
                Phone = "2222"
            };

            _contactRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(existing);

            _contactRepositoryMock
                .Setup(r => r.EmailExistsAsync(dto.Email, id))
                .ReturnsAsync(true);


            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.UpdateAsync(id, dto));

            Assert.Equal("Já existe outro contato com este e-mail.", ex.Message);

            _contactRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_DeveRetornarFalse_QuandoContatoNaoExiste()
        {

            var id = Guid.NewGuid();

            _contactRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Contact?)null);


            var result = await _service.DeleteAsync(id);

            Assert.False(result);
            _contactRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_DeveRetornarTrue_QuandoContatoExiste()
        {

            var id = Guid.NewGuid();

            var existing = new Contact
            {
                Id = id,
                Name = "Teste",
                Email = "teste@teste.com",
                Phone = "1111"
            };

            _contactRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(existing);


            var result = await _service.DeleteAsync(id);


            Assert.True(result);
            _contactRepositoryMock.Verify(r => r.DeleteAsync(existing), Times.Once);
        }
    }
}
