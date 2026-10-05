using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore;
using DeskFlow.API.Data;
using DeskFlow.API.Repositories;
using DeskFlow.API.Services;
using DeskFlow.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DeskFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IChamadoRepository, ChamadoRepository>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<ChamadoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
