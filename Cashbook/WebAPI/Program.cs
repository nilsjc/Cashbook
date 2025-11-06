using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using WebAPI.database;
using WebAPI.endpoints;
using WebAPI.validators;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:4711");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextFactory<CashBookDbContext>(options =>
    options.UseSqlite("Data Source=cashbook.db"));
builder.Services.AddScoped<IValidator<PostAccountDTO>, AccountCreateValidator>();
builder.Services.AddScoped<IDatabaseService, EFDatabaseService>();
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);
}

app.UseHttpsRedirection();

// Create database if not exists. This is bad practice and used only for demo purposes.
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<CashBookDbContext>>();
        using var db = factory.CreateDbContext();
        logger.LogInformation("Applying EF Core migrations (creating DB if missing)...");
        db.Database.EnsureCreated();
        logger.LogInformation("Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying migrations on startup.");
        throw;
    }
}

// register API endpoints
app.RegisterAccountEndpoints();
app.RegisterTransactionEndpoints();

app.Run();
