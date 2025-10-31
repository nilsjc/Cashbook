using FluentValidation;
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
builder.Services.AddScoped<IValidator<AccountDTO>, AccountCreateValidator>();
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

// register API endpoints
app.RegisterAccountEndpoints();

app.Run();
