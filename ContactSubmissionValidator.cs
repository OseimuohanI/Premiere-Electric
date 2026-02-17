// ContactSubmissionValidator.cs - FluentValidation
using FluentValidation;
using PremierElectric.Api.DTOs;

namespace PremierElectric.Api.Validators
{
    public class ContactSubmissionValidator : AbstractValidator<ContactSubmissionDto>
    {
        public ContactSubmissionValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .Length(2, 100).WithMessage("Full name must be between 2 and 100 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Full name can only contain letters and spaces");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be a valid email address");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(\d{3}-\d{3}-\d{4})?$")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Phone number must be in format XXX-XXX-XXXX");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required")
                .Length(5, 150).WithMessage("Subject must be between 5 and 150 characters");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required")
                .Length(10, 5000).WithMessage("Message must be between 10 and 5000 characters");

            RuleFor(x => x.ServiceCategory)
                .Must(x => string.IsNullOrEmpty(x) || IsValidServiceCategory(x))
                .When(x => !string.IsNullOrEmpty(x.ServiceCategory))
                .WithMessage("Service category is not valid");

            RuleFor(x => x.PreferredContact)
                .Must(x => string.IsNullOrEmpty(x) || x == "email" || x == "phone")
                .When(x => !string.IsNullOrEmpty(x.PreferredContact))
                .WithMessage("Preferred contact must be 'email' or 'phone'");
        }

        private bool IsValidServiceCategory(string category)
        {
            var validCategories = new[] { "residential", "commercial", "maintenance", "equipment", "other" };
            return validCategories.Contains(category.ToLower());
        }
    }
}