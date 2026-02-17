using PremierElectric.Api.DTOs;
using System.Text.RegularExpressions;

namespace PremierElectric.Api.Services
{
    /// <summary>
    /// Service for handling chatbot logic and response generation
    /// </summary>
    public interface IChatService
    {
        Task<ChatMessageResponseDto> ProcessMessageAsync(ChatMessageRequestDto request);
        Task<ChatSession?> GetSessionAsync(string sessionId);
        Task SaveSessionAsync(ChatSession session);
    }

    public class ChatService : IChatService
    {
        // In-memory session storage (use Redis or database in production)
        private static readonly Dictionary<string, ChatSession> _sessions = new();
        private static readonly object _sessionLock = new();
        private readonly ILogger<ChatService> _logger;

        public ChatService(ILogger<ChatService> logger)
        {
            _logger = logger;
        }

        public async Task<ChatMessageResponseDto> ProcessMessageAsync(ChatMessageRequestDto request)
        {
            try
            {
                // Get or create session
                var sessionId = request.SessionId ?? Guid.NewGuid().ToString();
                var session = await GetOrCreateSessionAsync(sessionId);

                // Store user message
                var userMessage = new ChatMessage
                {
                    Content = request.Message,
                    IsFromUser = true,
                    Timestamp = DateTime.UtcNow
                };
                session.Messages.Add(userMessage);

                // Generate bot response
                var botResponse = GenerateResponse(request.Message.ToLowerInvariant(), session);
                
                // Store bot message
                var botMessage = new ChatMessage
                {
                    Content = botResponse.BotResponse,
                    IsFromUser = false,
                    Timestamp = DateTime.UtcNow
                };
                session.Messages.Add(botMessage);

                // Save session
                await SaveSessionAsync(session);

                botResponse.SessionId = sessionId;
                botResponse.Success = true;
                botResponse.Message = "Response generated successfully";

                _logger.LogInformation("Processed chat message for session {SessionId}", sessionId);
                
                return botResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return new ChatMessageResponseDto
                {
                    Success = false,
                    Message = "Error processing your message",
                    BotResponse = "I'm sorry, I encountered an error. Please try again or contact us directly at (555) 123-4567.",
                    SessionId = request.SessionId ?? string.Empty
                };
            }
        }

        private ChatMessageResponseDto GenerateResponse(string userMessage, ChatSession session)
        {
            var response = new ChatMessageResponseDto
            {
                Timestamp = DateTime.UtcNow
            };

            // Greeting patterns
            if (Regex.IsMatch(userMessage, @"\b(hello|hi|hey|good morning|good afternoon|good evening)\b"))
            {
                response.BotResponse = session.Messages.Count <= 2
                    ? "Hello! Welcome to Premiere Electric. I'm here to help you with any questions about our electrical services. What can I assist you with today?"
                    : "Hello again! How else can I help you?";
                
                response.SuggestedActions = GetServiceSuggestions();
                return response;
            }

            // Services inquiry
            if (Regex.IsMatch(userMessage, @"\b(service|what do you do|what do you offer|capabilities)\b"))
            {
                response.BotResponse = "We offer comprehensive electrical services:\n\n" +
                    "🏠 Residential Wiring - Complete home electrical installations\n" +
                    "🏢 Commercial Electrical - Office and business solutions\n" +
                    "🔧 Maintenance & Repairs - Regular upkeep and emergency fixes\n" +
                    "⚡ Equipment Installation - Industrial and commercial equipment\n\n" +
                    "Would you like more details about any specific service?";
                
                response.SuggestedActions = new List<QuickReplyOption>
                {
                    new() { Label = "Residential Services", Value = "Tell me about residential services" },
                    new() { Label = "Commercial Services", Value = "Tell me about commercial services" },
                    new() { Label = "Get a Quote", Value = "I need a quote" }
                };
                return response;
            }

            // Residential services
            if (Regex.IsMatch(userMessage, @"\b(residential|home|house|domestic)\b"))
            {
                response.BotResponse = "Our residential services include:\n\n" +
                    "• Complete home wiring and rewiring\n" +
                    "• Panel upgrades and circuit breaker installation\n" +
                    "• Lighting design and installation\n" +
                    "• Outlet and switch installation\n" +
                    "• Smart home integration\n" +
                    "• Safety inspections\n\n" +
                    "All work is done by licensed electricians with a satisfaction guarantee!";
                
                response.SuggestedActions = GetQuoteSuggestions();
                return response;
            }

            // Commercial services
            if (Regex.IsMatch(userMessage, @"\b(commercial|business|office|industrial)\b"))
            {
                response.BotResponse = "We specialize in commercial electrical solutions:\n\n" +
                    "• Office electrical design and installation\n" +
                    "• Data center power systems\n" +
                    "• Emergency lighting systems\n" +
                    "• Energy-efficient LED retrofits\n" +
                    "• Preventive maintenance programs\n" +
                    "• 24/7 commercial support\n\n" +
                    "We work with businesses of all sizes!";
                
                response.SuggestedActions = GetQuoteSuggestions();
                return response;
            }

            // Pricing inquiry
            if (Regex.IsMatch(userMessage, @"\b(price|cost|quote|estimate|how much|pricing|rate|fee)\b"))
            {
                response.BotResponse = "Our pricing varies based on the scope and complexity of the project. We offer:\n\n" +
                    "💰 Basic Plan: $99/month - Ideal for homeowners\n" +
                    "💎 Standard Plan: $199/month - Perfect for small businesses\n" +
                    "🌟 Premium Plan: $299/month - Comprehensive coverage\n\n" +
                    "For custom projects, we provide FREE estimates! Would you like to request a quote?";
                
                response.SuggestedActions = new List<QuickReplyOption>
                {
                    new() { Label = "Request Quote", Value = "I need a quote" },
                    new() { Label = "View Plans", Value = "Tell me about your plans" },
                    new() { Label = "Call Now", Value = "What's your phone number?" }
                };
                return response;
            }

            // Emergency services
            if (Regex.IsMatch(userMessage, @"\b(emergency|urgent|immediate|asap|right now|help)\b"))
            {
                response.BotResponse = "⚡ EMERGENCY SERVICE AVAILABLE 24/7 ⚡\n\n" +
                    "For immediate emergency assistance, please call us directly at:\n" +
                    "📞 (555) 123-4567\n\n" +
                    "Our emergency team responds to:\n" +
                    "• Power outages\n" +
                    "• Electrical fires\n" +
                    "• Sparking outlets\n" +
                    "• Tripped breakers\n" +
                    "• Any safety hazards\n\n" +
                    "Don't wait - call now for immediate help!";
                
                return response;
            }

            // Contact information
            if (Regex.IsMatch(userMessage, @"\b(contact|reach|phone|email|address|location|hours|open)\b"))
            {
                var contactInfo = "📞 Phone: (555) 123-4567\n" +
                    "📧 Email: info@premierelectric.com\n" +
                    "🕐 Hours: Monday-Friday, 8AM-6PM\n" +
                    "⚡ Emergency Service: 24/7\n\n";

                if (Regex.IsMatch(userMessage, @"\b(hours|open|when)\b"))
                {
                    response.BotResponse = "We're open Monday through Friday, 8AM to 6PM. " +
                        "However, we offer 24/7 emergency service for urgent electrical issues!\n\n" + contactInfo +
                        "Would you like to schedule an appointment?";
                }
                else
                {
                    response.BotResponse = "You can reach us at:\n\n" + contactInfo +
                        "Feel free to call, email, or fill out our contact form!";
                }
                
                response.SuggestedActions = new List<QuickReplyOption>
                {
                    new() { Label = "Fill Contact Form", Value = "I want to fill the contact form" },
                    new() { Label = "Emergency Service", Value = "I have an emergency" }
                };
                return response;
            }

            // Appointment/booking
            if (Regex.IsMatch(userMessage, @"\b(appointment|schedule|book|booking|visit|come|when can)\b"))
            {
                response.BotResponse = "I'd be happy to help you schedule an appointment! " +
                    "Please fill out our contact form with your preferred date and time, " +
                    "or call us at (555) 123-4567 to speak with our scheduling team.\n\n" +
                    "We typically offer appointments:\n" +
                    "• Monday-Friday: 8AM-6PM\n" +
                    "• Same-day service available\n" +
                    "• Emergency calls: 24/7\n\n" +
                    "What type of service do you need?";
                
                response.SuggestedActions = GetServiceSuggestions();
                return response;
            }

            // Thank you
            if (Regex.IsMatch(userMessage, @"\b(thank|thanks|appreciate)\b"))
            {
                response.BotResponse = "You're very welcome! Is there anything else I can help you with today?";
                response.SuggestedActions = GetServiceSuggestions();
                return response;
            }

            // Default response
            response.BotResponse = "I'd be happy to help! I can answer questions about:\n\n" +
                "• Our electrical services\n" +
                "• Pricing and quotes\n" +
                "• Business hours and contact info\n" +
                "• Scheduling appointments\n" +
                "• Emergency services\n\n" +
                "What would you like to know more about?";
            
            response.SuggestedActions = GetServiceSuggestions();
            return response;
        }

        private List<QuickReplyOption> GetServiceSuggestions()
        {
            return new List<QuickReplyOption>
            {
                new() { Label = "View Services", Value = "What services do you offer?" },
                new() { Label = "Get Pricing", Value = "How much does it cost?" },
                new() { Label = "Contact Us", Value = "How can I contact you?" }
            };
        }

        private List<QuickReplyOption> GetQuoteSuggestions()
        {
            return new List<QuickReplyOption>
            {
                new() { Label = "Request Quote", Value = "I need a quote" },
                new() { Label = "Schedule Visit", Value = "Schedule an appointment" },
                new() { Label = "Call Now", Value = "What's your phone number?" }
            };
        }

        private async Task<ChatSession> GetOrCreateSessionAsync(string sessionId)
        {
            return await Task.Run(() =>
            {
                lock (_sessionLock)
                {
                    if (_sessions.TryGetValue(sessionId, out var session))
                    {
                        return session;
                    }

                    var newSession = new ChatSession
                    {
                        SessionId = sessionId,
                        StartedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    _sessions[sessionId] = newSession;
                    return newSession;
                }
            });
        }

        public async Task<ChatSession?> GetSessionAsync(string sessionId)
        {
            return await Task.Run(() =>
            {
                lock (_sessionLock)
                {
                    return _sessions.TryGetValue(sessionId, out var session) ? session : null;
                }
            });
        }

        public async Task SaveSessionAsync(ChatSession session)
        {
            await Task.Run(() =>
            {
                lock (_sessionLock)
                {
                    _sessions[session.SessionId] = session;
                    
                    // Clean up old sessions (older than 24 hours)
                    var expiredSessions = _sessions
                        .Where(s => DateTime.UtcNow - s.Value.StartedAt > TimeSpan.FromHours(24))
                        .Select(s => s.Key)
                        .ToList();

                    foreach (var expired in expiredSessions)
                    {
                        _sessions.Remove(expired);
                    }
                }
            });
        }
    }
}