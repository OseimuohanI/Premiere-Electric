# Chatbot Backend Implementation

## Overview
The chatbot backend provides intelligent responses to user inquiries about electrical services, pricing, contact information, and scheduling.

## Architecture

### Components

1. **ChatMessageDto.cs** - Data transfer objects
   - `ChatMessageRequestDto` - User message input
   - `ChatMessageResponseDto` - Bot response with suggested actions
   - `ChatSession` - Session tracking with message history
   - `QuickReplyOption` - Suggested action buttons

2. **ChatService.cs** - Business logic
   - Pattern-based message analysis using regex
   - Context-aware responses
   - Session management with 24-hour expiration
   - Quick reply suggestions
   - In-memory session storage (easily upgradeable to Redis)

3. **ChatController.cs** - API endpoints
   - `POST /api/chat/message` - Process user messages
   - `GET /api/chat/session/{sessionId}` - Retrieve session history
   - `GET /api/chat/health` - Health check

## Features

### Intelligent Response Patterns

The chatbot recognizes and responds to:

- **Greetings** - "hello", "hi", "hey"
- **Services** - "what do you do", "services", "capabilities"
- **Residential** - Home wiring, installations, smart home
- **Commercial** - Business electrical, office solutions
- **Pricing** - Cost estimates, quotes, plans
- **Emergency** - Urgent 24/7 support
- **Contact** - Hours, phone, email, location
- **Scheduling** - Appointments, bookings
- **Thank you** - Polite acknowledgments

### Session Management

- Unique session IDs for conversation tracking
- Message history stored per session
- Automatic cleanup of 24+ hour old sessions
- Thread-safe in-memory storage

### Quick Replies

Dynamic suggested actions based on context:
- "View Services"
- "Get Pricing"
- "Request Quote"
- "Contact Us"
- "Emergency Service"

## Integration

### Frontend Integration

The frontend (`index.html`) now:
1. Calls `/api/chat/message` endpoint
2. Stores session ID in `sessionStorage`
3. Falls back to local responses if API unavailable
4. Shows typing indicator during processing

### Program.cs Registration

```csharp
builder.Services.AddScoped<IChatService, ChatService>();
```

## API Usage

### Send Message

```bash
POST /api/chat/message
Content-Type: application/json

{
  "message": "What services do you offer?",
  "sessionId": "optional-session-id",
  "userContext": "/index.html"
}
```

Response:
```json
{
  "success": true,
  "message": "Response generated successfully",
  "botResponse": "We offer comprehensive electrical services...",
  "sessionId": "abc-123-def",
  "suggestedActions": [
    { "label": "Residential Services", "value": "Tell me about residential" },
    { "label": "Get Quote", "value": "I need a quote" }
  ],
  "timestamp": "2026-01-27T10:30:00Z"
}
```

## Future Enhancements

### Easy Upgrades

1. **AI Integration**
   - Replace pattern matching with OpenAI GPT
   - Add `ILlmService` interface
   - Configure API keys in `appsettings.json`

2. **Persistent Storage**
   - Swap in-memory storage for Redis
   - Add Entity Framework models for chat history
   - Enable analytics and reporting

3. **Advanced Features**
   - Sentiment analysis
   - Multi-language support
   - Voice input/output
   - File attachments
   - Agent handoff to human support

4. **Analytics**
   - Track popular questions
   - Measure response satisfaction
   - Identify knowledge gaps

## Testing

```bash
# Test chatbot endpoint
curl -X POST http://localhost:5000/api/chat/message \
  -H "Content-Type: application/json" \
  -d '{"message":"What are your hours?"}'

# Check health
curl http://localhost:5000/api/chat/health
```

## Production Considerations

1. **Rate Limiting** - Add rate limiting to prevent abuse
2. **Caching** - Cache common responses
3. **Monitoring** - Log conversation metrics
4. **Security** - Validate/sanitize all user input
5. **Scalability** - Use Redis for session storage in multi-instance deployments

## Dependencies

- No additional NuGet packages required for basic implementation
- Optional: `StackExchange.Redis` for Redis integration
- Optional: `OpenAI` or `Azure.AI.OpenAI` for GPT integration
