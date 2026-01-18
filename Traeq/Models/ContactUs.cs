namespace Traeq.Models
{
    public class ContactUs : BaseEntity
    {
        public string? NameFirst { get; set; }
        public string? NameLast { get; set; }
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }

    }
}
