// ContactController.cs - API Controller
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using PremierElectric.Application.DTOs;
using PremierElectric.Application.Services;

namespace PremierElectric.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IValidator<ContactSubmissionDto> _validator;
        private readonly ILogger<ContactController> _logger;

        public ContactController(
            IContactService contactService,
            IValidator<ContactSubmissionDto> validator,
            ILogger<ContactController> logger)
        {
            _contactService = contactService;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// Submit a new contact form
        /// </summary>
        /// <param name="dto">Contact submission data</param>
        /// <returns>Contact response with ticket ID</returns>
        [HttpPost("submit")]
        public async Task<ActionResult<ContactResponseDto>> SubmitContact([FromBody] ContactSubmissionDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new ContactResponseDto
                {
                    Success = false,
                    Message = "Request body cannot be empty",
                    Errors = new Dictionary<string, string> { { "general", "No data provided" } }
                });
            }

            // Validate input
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.First().ErrorMessage);

                return BadRequest(new ContactResponseDto
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                });
            }

            _logger.LogInformation($"Processing contact submission from {dto.Email}");

            // Process submission
            var response = await _contactService.SubmitContactFormAsync(dto);

            if (response.Success)
            {
                return Ok(response);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        /// <summary>
        /// Get contact submission by ID
        /// </summary>
        /// <param name="id">Contact submission ID</param>
        /// <returns>Contact submission details</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetContact(Guid id)
        {
            var contact = await _contactService.GetContactByIdAsync(id);
            if (contact == null)
            {
                return NotFound(new { message = "Contact submission not found" });
            }

            return Ok(contact);
        }
    }
}
