using Microsoft.AspNetCore.Mvc;
using PremierElectric.Api.DTOs;
using PremierElectric.Api.Services;

namespace PremierElectric.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        /// <summary>
        /// Process a chat message and return bot response
        /// </summary>
        /// <param name="request">Chat message request with user message and optional session ID</param>
        /// <returns>Bot response with suggested actions</returns>
        [HttpPost("message")]
        [ProducesResponseType(typeof(ChatMessageResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ChatMessageResponseDto>> SendMessage([FromBody] ChatMessageRequestDto request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest(new ChatMessageResponseDto
                    {
                        Success = false,
                        Message = "Message cannot be empty",
                        BotResponse = "Please provide a message."
                    });
                }

                // Trim and validate message length
                request.Message = request.Message.Trim();
                if (request.Message.Length > 500)
                {
                    return BadRequest(new ChatMessageResponseDto
                    {
                        Success = false,
                        Message = "Message is too long",
                        BotResponse = "Please keep your message under 500 characters."
                    });
                }

                _logger.LogInformation("Processing chat message: {Message}", request.Message);

                // Process message
                var response = await _chatService.ProcessMessageAsync(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, new ChatMessageResponseDto
                {
                    Success = false,
                    Message = "An error occurred processing your message",
                    BotResponse = "I apologize, but I'm having technical difficulties. Please try again or contact us at (555) 123-4567."
                });
            }
        }

        /// <summary>
        /// Get chat session history (optional - for admin/debugging)
        /// </summary>
        /// <param name="sessionId">Session ID to retrieve</param>
        /// <returns>Chat session with message history</returns>
        [HttpGet("session/{sessionId}")]
        [ProducesResponseType(typeof(ChatSession), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChatSession>> GetSession(string sessionId)
        {
            try
            {
                var session = await _chatService.GetSessionAsync(sessionId);
                
                if (session == null)
                {
                    return NotFound(new { message = "Session not found" });
                }

                return Ok(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving session {SessionId}", sessionId);
                return StatusCode(500, new { message = "Error retrieving session" });
            }
        }

        /// <summary>
        /// Health check endpoint for chatbot service
        /// </summary>
        /// <returns>Status of chatbot service</returns>
        [HttpGet("health")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "healthy",
                service = "chatbot",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
