namespace gus_API.Models.DTOs.AccountDTOs
{
    public class EpDetailDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? AccountNumber { get; set; }

        public int? WalletId { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string Inn { get; set; } = null!;

        public string ShortName { get; set; } = null!;

        public string Bik { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Ogrnip { get; set; } = null!;

        public string LegalAddress { get; set; } = null!;

        public string MagazinName { get; set; } = null!;
    }
}
