using Application.UseCases;
using Domain.Repositories;
using Domain.Services;
using Infra.Repository;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IGameOfLifeRepository, GameOfLifeRepository>();
builder.Services.AddScoped<IGameOfLifeUseCase, GameOfLifeUseCase>();
builder.Services.AddScoped<IGameOfLifeService, GameOfLifeService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
//Habilitate CORS
app.UseCors("AllowFrontend");

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
