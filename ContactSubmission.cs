// ContactSubmission.cs - Domain Entity
namespace PremierElectric.Domain.Entities
{
    public class ContactSubmission
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string ServiceCategory { get; set; }
        public string PreferredContact { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public ContactStatus Status { get; set; } = ContactStatus.Received;
        public string AdminNotes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum ContactStatus
    {
        Received = 0,
        InReview = 1,
        Responded = 2,
        Archived = 3
    }
}