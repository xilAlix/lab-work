using Microsoft.EntityFrameworkCore;
using Variant10.Models;
using Variant10.Services;

var builder = WebApplication.CreateBuilder(args);
// Добавляем сервис контекста базы данных
builder.Services.AddDbContext<ProductContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Добавление сервисов
builder.Services.AddScoped<FoodProductService>();
builder.Services.AddScoped<FacturingCompanyService>();
builder.Services.AddScoped<FoodProductionService>();

// Add services to the container.

builder.Services.AddControllers();
// ... после builder.Services.AddControllers(); ...
// Добавляем сервисы CORS
builder.Services.AddCors(options =>
{
options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Разрешить запросы с любых источников (для учебных целей)
        .AllowAnyMethod() // Разрешить любые HTTP-методы (GET, POST, ...)
        .AllowAnyHeader(); // Разрешить любые заголовки
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<FoodProductService>();

var app = builder.Build();
app.UseCors("AllowAll");
app.UseDefaultFiles(); // Ищет файлы по умолчанию (index.html)
app.UseStaticFiles(); // Подключает статические файлы

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
