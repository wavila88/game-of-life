using Api.WebSocket;
using Application.UseCases;
using Domain.Repositories;
using Domain.Services;
using Infra.Repository;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(
        builder.Configuration.GetValue<string>("Redis:ConnectionString")
    )
);
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
var allowedOrigins = builder.Configuration.GetValue<string>("CORS:AllowedOrigins");
var originsArray = allowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(originsArray)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
//Web socket configuration
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
//Habilitate CORS
app.UseCors("AllowFrontend");

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHub<GameOfLifeHub>("/gameoflifehub");

app.Run();
