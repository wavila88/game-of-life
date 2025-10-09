using Application.UseCases;
using Domain.Repositories;
using Domain.Services;
using Infra.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IGameOfLifeRepository, GameOfLifeRepository>();
builder.Services.AddScoped<IGameOfLifeUseCase, GameOfLifeUseCase>();
builder.Services.AddScoped<IGameOfLifeService, GameOfLifeService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
