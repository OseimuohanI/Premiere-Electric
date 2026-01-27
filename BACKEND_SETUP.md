# Premiere Electric - .NET Backend Setup Guide

## Project Overview
This is an ASP.NET Core 7+ REST API backend for the Premiere Electric website. It handles contact form submissions with email notifications, data validation, and error handling.

## Architecture
```
PremierElectric.API/
├── PremierElectric.API/                (Main API Project)
│   ├── Controllers/
│   │   └── ContactController.cs
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── PremierElectric.API.csproj
│
├── PremierElectric.Application/        (Business Logic)
│   ├── Services/
│   │   ├── IContactService.cs
│   │   ├── ContactService.cs
│   │   ├── IEmailService.cs
│   │   └── EmailService.cs
│   ├── DTOs/
│   │   ├── ContactSubmissionDto.cs
│   │   └── ContactResponseDto.cs
│   └── Validators/
│       └── ContactSubmissionValidator.cs
│
├── PremierElectric.Domain/             (Core Domain Models)
│   ├── Entities/
│   │   └── ContactSubmission.cs
│   └── Constants/
│       └── ServiceCategories.cs
│
├── PremierElectric.Infrastructure/     (Data & External Services)
│   ├── Data/
│   │   └── PremierElectricDbContext.cs
│   └── Migrations/
│
└── README.md                           (This file)
```

## Prerequisites
- .NET 7 SDK or higher
- Visual Studio 2022 or Visual Studio Code
- SQL Server 2019+ or PostgreSQL 12+
- SMTP Server credentials (Gmail, SendGrid, or custom SMTP)

## Installation & Setup

### 1. Create Solution and Projects
```bash
# Create solution
dotnet new sln -n PremierElectric

# Create projects
dotnet new classlib -n PremierElectric.Domain
dotnet new classlib -n PremierElectric.Application
dotnet new classlib -n PremierElectric.Infrastructure
dotnet new webapi -n PremierElectric.API

# Add projects to solution
dotnet sln add PremierElectric.Domain
dotnet sln add PremierElectric.Application
dotnet sln add PremierElectric.Infrastructure
dotnet sln add PremierElectric.API
```

### 2. Add NuGet Packages
```bash
cd PremierElectric.API
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions

cd ../PremierElectric.Application
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package FluentValidation

cd ../PremierElectric.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### 3. Configure CORS (in Program.cs)
```csharp
builder.Services.AddCors(options => options.AddPolicy("AllowFrontend",
    builder => builder
        .WithOrigins("http://localhost:3000", "http://localhost", "http://oseimuohans-macbook-air.local")
        .AllowAnyMethod()
        .AllowAnyHeader()));

app.UseCors("AllowFrontend");
```

### 4. Database Connection
Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=PremierElectricDB;Trusted_Connection=true;"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password",
    "AdminEmail": "admin@premierelectric.com"
  }
}
```

### 5. Create Database and Tables
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## API Endpoints

### POST /api/contact/submit
Submit a new contact form.

**Request:**
```json
{
  "fullName": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "555-123-4567",
  "subject": "Electrical Service Inquiry",
  "message": "I need electrical wiring for my new home renovation.",
  "serviceCategory": "residential",
  "preferredContact": "email"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Your message has been sent successfully",
  "ticketId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response (400 Bad Request):**
```json
{
  "success": false,
  "errors": {
    "email": "Invalid email format",
    "message": "Message must be at least 10 characters"
  }
}
```

## Email Configuration

### Using Gmail
1. Enable 2-Factor Authentication
2. Create App Password
3. Use in `appsettings.json`

### Using SendGrid (Alternative)
```csharp
// Install: dotnet add package SendGrid
var sendGridClient = new SendGridClient(apiKey);
await sendGridClient.SendEmailAsync(from, to, subject, plainTextContent, htmlContent);
```

## Development

### Run API
```bash
dotnet run
```

API will be available at: `https://localhost:7000` or `http://localhost:5000`

### Test Endpoint
```bash
curl -X POST http://localhost:5000/api/contact/submit \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Test User",
    "email": "test@example.com",
    "subject": "Test Subject",
    "message": "This is a test message from curl."
  }'
```

## Deployment

### Docker (Recommended)
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore && dotnet build -c Release

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/PremierElectric.API/bin/Release/net7.0 .
EXPOSE 80
ENTRYPOINT ["dotnet", "PremierElectric.API.dll"]
```

### Azure App Service
```bash
az group create -n PremierElectricRG -l eastus
az appservice plan create -n PremierElectricPlan -g PremierElectricRG --sku F1
az webapp create -g PremierElectricRG -p PremierElectricPlan -n premierelectric-api
dotnet publish -c Release
az webapp deployment source config-zip -g PremierElectricRG -n premierelectric-api --src Release.zip
```

## Monitoring & Logging
- Application Insights (Azure)
- Serilog (local development)
- ELK Stack (production alternative)

## Support
For issues or questions, contact: support@premierelectric.com
