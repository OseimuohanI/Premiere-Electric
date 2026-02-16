https://charming-paprenjak-ca099d.netlify.app/

## Frontend API Base URL

The frontend calls the backend using a configurable base URL. Set it by editing the meta tag in [index.html](index.html):

```html
<meta name="api-base-url" content="https://your-api-domain.com">
```

Leave it blank to use same-origin (useful when hosting the frontend and API together).

## Netlify Functions (Node) API

Netlify Functions are implemented in [netlify/functions/api.js](netlify/functions/api.js) and mapped via [netlify.toml](netlify.toml). This keeps frontend URLs the same:

- `POST /api/contact/submit`
- `POST /api/chat/message`

### Environment Variables

Set these in Netlify (Site settings -> Environment variables):

- `RESEND_API_KEY`
- `SUPABASE_URL` (example: https://csihmcsbvybtmtzxunld.supabase.co)
- `SUPABASE_KEY`
- `CONTACT_FROM_EMAIL` (default: contact@premierelectric.com)
- `CONTACT_ADMIN_EMAIL` (default: osemekeme@gmail.com)

### Supabase Setup

Create a Supabase project and a table named `contact_submissions` with the following SQL:

```sql
create table if not exists public.contact_submissions (
	id uuid primary key default gen_random_uuid(),
	full_name text not null,
	email text not null,
	phone_number text,
	subject text not null,
	message text not null,
	service_category text,
	preferred_contact text,
	ticket_id text not null,
	submitted_at timestamptz not null default now()
);
```

## Email Settings

Email delivery is disabled by default. Enable it and provide SMTP settings via environment variables or user secrets:

- `EmailSettings__Enabled=true`
- `EmailSettings__SmtpServer=smtp.gmail.com`
- `EmailSettings__SmtpPort=587`
- `EmailSettings__EnableSsl=true`
- `EmailSettings__SenderEmail=your-email@gmail.com`
- `EmailSettings__SenderPassword=your-app-specific-password`
- `EmailSettings__AdminEmail=admin@premierelectric.com`

Avoid committing real credentials to source control.

## Namespaces

API types use `PremierElectric.Api.*` while infrastructure and domain types remain in their own namespaces:

- `PremierElectric.Api.Controllers`
- `PremierElectric.Api.Services`
- `PremierElectric.Api.DTOs`
- `PremierElectric.Api.Validators`
- `PremierElectric.Infrastructure.Data`
- `PremierElectric.Domain.Entities`
