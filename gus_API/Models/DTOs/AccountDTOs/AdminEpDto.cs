namespace gus_API.Models.DTOs.AdminDTOs
{
    public class AdminEpDto
    {
        public int Id { get; set; }
        public string? AccountNumber { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public string LegalAddress { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }
}
