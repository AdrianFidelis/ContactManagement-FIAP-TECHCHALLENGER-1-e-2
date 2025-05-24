using ContactManagement.Domain.Repositories;
using ContactManagement.Infrastructure.Data;
using ContactManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Adiciona o DbContext com banco em memória
builder.Services.AddDbContext<ContactDbContext>(options =>
    options.UseInMemoryDatabase("ContactDb"));

// 🧠 Injeta o repositório
builder.Services.AddScoped<IContactRepository, ContactRepository>();

// 🔧 Add controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contact.Persistence.API", Version = "v1" });
});

var app = builder.Build();

// 🚀 Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
