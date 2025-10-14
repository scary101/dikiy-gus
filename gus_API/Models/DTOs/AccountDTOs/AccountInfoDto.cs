namespace gus_API.Models.DTOs.AdminDTOs
{
    public class AccountInfoDto
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string Role { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool Isep { get; set; }

    }
}
