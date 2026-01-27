# Quick Reference Guide - Premiere Electric Website

## 🚀 Quick Start

### Frontend (HTML/CSS/JavaScript)
```bash
# No build required - open index.html in browser
# Files are ready to deploy to any web server
```

### Backend (.NET)
```bash
# Create projects
dotnet new sln -n PremierElectric
dotnet new webapi -n PremierElectric.API
dotnet new classlib -n PremierElectric.Application
dotnet new classlib -n PremierElectric.Infrastructure  
dotnet new classlib -n PremierElectric.Domain

# Add packages
cd PremierElectric.API
dotnet add package FluentValidation
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# Run
dotnet run
# API available at http://localhost:5000
```

---

## 📋 Important Files

| File | Purpose |
|------|---------|
| `index.html` | Main website home page |
| `about.html` | Company information page |
| `main.css` | All styling |
| `logos/` | SVG logos & favicon |
| `BACKEND_SETUP.md` | Backend installation guide |
| `IMPLEMENTATION_COMPLETE.md` | Full implementation details |

---

## 🔗 API Endpoint

**URL:** `POST /api/contact/submit`

**Example Request:**
```javascript
fetch('/api/contact/submit', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    fullName: "John Doe",
    email: "john@example.com",
    phoneNumber: "555-123-4567",
    subject: "Service Inquiry",
    message: "I need electrical work done.",
    serviceCategory: "residential",
    preferredContact: "email"
  })
})
```

**Success Response (200):**
```json
{
  "success": true,
  "message": "Your message has been sent successfully",
  "ticketId": "550e8400-e29b-41d4-a716-446655440000"
}
```

---

## 🎨 Design Assets

### Colors
- Primary Blue: `#007bff`
- Dark Blue: `#1a3a52`
- Electric Orange: `#ff9500`
- Text: `#555555`
- Light: `#efefef`

### Fonts
- Default: Arial, sans-serif
- All headings: Bold 600-700 weight

### Icons
- Font Awesome 6.4.0
- `fas` = Solid icons
- `fab` = Brand/Social icons

---

## 📱 Responsive Breakpoints

- Desktop: 1024px+
- Tablet: 576px - 1023px
- Mobile: < 576px

---

## 🔐 Security Checklist

- [ ] HTTPS enabled
- [ ] CORS properly configured for production domain
- [ ] Input validation on both frontend & backend
- [ ] SQL injection protection (Entity Framework)
- [ ] XSS protection (HTML encoding)
- [ ] CSRF token protection (if needed)
- [ ] Rate limiting on API endpoints
- [ ] Email credentials secured in environment variables

---

## 📧 Email Configuration

### Gmail Setup
1. Enable 2-Factor Authentication
2. Create App Password
3. Update `appsettings.json`:
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password",
    "AdminEmail": "admin@premierelectric.com"
  }
}
```

### SendGrid Alternative
```csharp
// Add package: dotnet add package SendGrid
var sendGridClient = new SendGridClient(apiKey);
await sendGridClient.SendEmailAsync(from, to, subject, plainText, html);
```

---

## 🧪 Testing

### Frontend Testing
- Test form validation
- Test responsive design
- Test browser compatibility
- Test links/navigation
- Test email submission

### Backend Testing
```bash
# Test API with curl
curl -X POST http://localhost:5000/api/contact/submit \
  -H "Content-Type: application/json" \
  -d '{"fullName":"Test","email":"test@example.com","subject":"Test","message":"Test message"}'

# Or use Postman/Thunder Client
```

---

## 📦 Deployment

### Frontend (Static Files)
1. Upload index.html, about.html, main.css to web server
2. Upload logos/ directory
3. Upload image/ directory
4. Configure CORS on frontend for backend API

### Backend (.NET)
```bash
# Publish for production
dotnet publish -c Release

# Deploy to Azure
az webapp deployment
```

---

## 🆘 Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| CORS error on form submit | Check backend CORS config includes frontend domain |
| Images not loading | Verify image/ directory is uploaded |
| Logo not showing | Check logos/ directory path and SVG file permissions |
| Email not sending | Verify SMTP credentials and port 587 is accessible |
| Bootstrap not working | Verify Bootstrap CDN link (5.3.0) |
| Font Awesome icons missing | Verify Font Awesome CDN (6.4.0) |
| About page 404 | Ensure about.html is deployed to web server |

---

## 📞 Quick Support

**Frontend Issues:**
- Check browser console (F12 → Console)
- Clear cache and reload
- Test in different browser

**Backend Issues:**
- Check API logs: `dotnet run` shows debug output
- Verify database connection
- Check SMTP credentials
- Review validation error messages

---

## 🎯 Performance Tips

- Images: Use optimized formats (JPEG for photos, PNG for graphics)
- CSS: Minify for production
- JavaScript: Move scripts to end of body
- Database: Add indices on frequently queried columns
- Email: Send asynchronously to not block requests
- API: Implement rate limiting

---

## 📚 Documentation Files

1. **BACKEND_SETUP.md** - Complete backend setup guide
2. **IMPLEMENTATION_COMPLETE.md** - Full implementation summary
3. **This file** - Quick reference

---

**Last Updated:** January 27, 2026
**Version:** 1.0 (Complete)
