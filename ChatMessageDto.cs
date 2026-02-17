namespace PremierElectric.Api.DTOs
{
    /// <summary>
    /// Request DTO for chat message submission
    /// </summary>
    public class ChatMessageRequestDto
    {
        public string Message { get; set; } = string.Empty;
        public string? SessionId { get; set; }
        public string? UserContext { get; set; } // Optional: page URL, previous interactions, etc.
    }

    /// <summary>
    /// Response DTO for chat bot replies
    /// </summary>
    public class ChatMessageResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string BotResponse { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public List<QuickReplyOption>? SuggestedActions { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Quick reply suggestions for user convenience
    /// </summary>
    public class QuickReplyOption
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Chat session model for tracking conversations
    /// </summary>
    public class ChatSession
    {
        public string SessionId { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public List<ChatMessage> Messages { get; set; } = new();
        public string? UserEmail { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Individual chat message record
    /// </summary>
    public class ChatMessage
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString();
        public string Content { get; set; } = string.Empty;
        public bool IsFromUser { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}