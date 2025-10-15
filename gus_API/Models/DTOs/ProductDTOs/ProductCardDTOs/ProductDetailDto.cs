namespace gus_API.Models.DTOs.ProductDTOs
{
    /// <summary>
    /// DTO для детальной информации о товаре.
    /// </summary>
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int? Stock { get; set; }
        public bool? IsActive { get; set; }
        public decimal Rating { get; set; }
        public int ReviewsCount { get; set; }
        public string? PhotoPath { get; set; }
        public DateTime? CreatedAt { get; set; }
        public ProductDetailCategoryDto? Category { get; set; }
        public ProductDetailEntrepreneurDto Entrepreneur { get; set; } = null!;
        public List<ProductDetailCharacteristicDto> Characteristics { get; set; } = new();
        public List<ProductDetailReviewDto> Reviews { get; set; } = new();
    }
    public class ProductDetailCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? ParentId { get; set; }
    }
    public class ProductDetailEntrepreneurDto
    {
        public int Id { get; set; }
        public string MagazinName { get; set; } = null!;
    }
    public class ProductDetailCharacteristicDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Value { get; set; }
        public string? Unit { get; set; }
    }
    public class ProductDetailReviewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? Rating { get; set; }
        public string? Text { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
