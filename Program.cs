// Program.cs - Startup configuration
using FluentValidation;
using PremierElectric.Api.DTOs;
using PremierElectric.Api.Services;
using PremierElectric.Api.Validators;
using PremierElectric.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PremierElectricDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ContactSubmissionValidator>();
builder.Services.AddScoped<IValidator<ContactSubmissionDto>, ContactSubmissionValidator>();

// Add Application Services
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IChatService, ChatService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://charming-paprenjak-ca099d.netlify.app",
            "https://oseimuohani.github.io",
            "http://localhost",
            "http://localhost:5500",
            "http://localhost:5000",
            "http://127.0.0.1:5500"
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Add Logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// Automatic migrations are disabled by default. To re-enable, set ENABLE_MIGRATIONS=true.
if (string.Equals(Environment.GetEnvironmentVariable("ENABLE_MIGRATIONS"), "true", StringComparison.OrdinalIgnoreCase))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PremierElectricDbContext>();
        dbContext.Database.Migrate();
    }
}

app.Run();
