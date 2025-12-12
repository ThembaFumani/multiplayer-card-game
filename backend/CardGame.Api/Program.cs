using System.Text.Json.Serialization;
using CardGame.Api.Data;
using CardGame.Api.Repositories;
using CardGame.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CardGameContext>(opts =>
    opts.UseInMemoryDatabase("CardGameDb"));
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameServices, GameService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.UseCors();

app.MapControllers();

app.Map("/error", (HttpContext ctx) =>
{
    return Results.Problem(
        statusCode: 500,
        title: "Unexpected error",
        detail: "An unexpected error occurred. Please try again.",
        instance: ctx.TraceIdentifier);
});

app.Run();
