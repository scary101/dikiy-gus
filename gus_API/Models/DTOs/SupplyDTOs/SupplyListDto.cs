namespace gus_API.Models.DTOs.SupplyDTOs
{
    public class SupplyListDto
    {
        public int Id { get; set; }
        public string EntrepreneurName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? ComletedAt { get; set; }
        public ManagerDto? Manager { get; set; } 
        public List<SupplyItemDto> Items { get; set; } = new();
    }
    public class SupplyItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
    }
    public class ManagerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string MiddleName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
