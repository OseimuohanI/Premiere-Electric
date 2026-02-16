## Premiere Electric

Premiere Electric is a full-stack web app for an electrical contractor with a static frontend and an ASP.NET Core API.

Live site: https://charming-paprenjak-ca099d.netlify.app/   or use https://oseimuohani.github.io/Premiere-Electric/

## Stack / Tech Used

- HTML, CSS, JavaScript (static frontend)
- ASP.NET Core 8 (C#) API
- Entity Framework Core (SQL Server)
- Railway (API hosting)
- Netlify (frontend hosting)

## Setup

### 1) Install dependencies

```bash
dotnet restore
```

### 2) Configure environment variables

Set these in your shell or hosting provider:

```
ConnectionStrings__DefaultConnection=<sql-connection-string>
EmailSettings__Enabled=true|false
EmailSettings__Host=<smtp-host>
EmailSettings__Port=587
EmailSettings__Username=<smtp-username>
EmailSettings__Password=<smtp-password>
EmailSettings__EnableSsl=true
EmailSettings__AdminEmail=admin@example.com
EmailSettings__FromEmail=from@example.com
```

### 3) Run the API

```bash
dotnet run
```

The API will listen on the port configured by `ASPNETCORE_URLS` or the default development port.

### 4) Run the frontend

Open `index.html` directly or serve it with any static server.
