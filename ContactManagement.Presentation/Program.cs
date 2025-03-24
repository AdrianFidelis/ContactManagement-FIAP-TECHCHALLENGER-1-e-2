using ContactManagement.Domain.Repositories;
using ContactManagement.Infrastructure.Data;
using ContactManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using ContactManagement.Application.Validators;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Entity Framework Core
builder.Services.AddDbContext<ContactDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro do repositório no contêiner DI
builder.Services.AddScoped<IContactRepository, ContactRepository>();

// Adicionando suporte a In-Memory Cache
builder.Services.AddMemoryCache();

// Registra os controladores
builder.Services.AddControllers();

// Registra FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Substitui RegisterValidatorsFromAssemblyContaining<T>()
builder.Services.AddValidatorsFromAssemblyContaining<ContactValidator>();

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ⬇️ MIGRATION EXECUTADA NA INICIALIZAÇÃO
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContactDbContext>();
    // Apenas migra se for um provider relacional
    if (db.Database.IsRelational())
    {
        db.Database.Migrate();
    }
    // ou .EnsureCreated() se você não estiver usando migrations
}

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }


