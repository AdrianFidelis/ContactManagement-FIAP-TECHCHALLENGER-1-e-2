using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Add controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contact.API", Version = "v1" });
});

// 🔗 HttpClient para chamar a API de persistência (porta correta: 5189)
builder.Services.AddHttpClient("PersistenceApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5189");
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
