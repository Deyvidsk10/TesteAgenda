using Agenda.Api.Profiles;
using Agenda.Api.Services.Contacts;
using Agenda.Api.Validators.Contacts;
using Agenda.Infrastructure.Data;
using Agenda.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper 
builder.Services.AddAutoMapper(typeof(ContactProfile).Assembly);

// DbContext
builder.Services.AddDbContext<AgendaDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// DI de Repository e Service
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();

builder.Services.AddControllers();

// Registra o FluentValidation
builder.Services.AddFluentValidationAutoValidation();      // ativa a validação automática via pipeline
builder.Services.AddFluentValidationClientsideAdapters();  // se quiser validação client-side (mais pro MVC/Razor, mas ok)

// Registra todos os validators do assembly onde está o CreateContactDtoValidator
builder.Services.AddValidatorsFromAssemblyContaining<CreateContactDtoValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

