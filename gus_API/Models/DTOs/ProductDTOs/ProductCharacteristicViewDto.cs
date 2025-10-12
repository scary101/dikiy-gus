namespace gus_API.Models.DTOs.ProductDTOs
{
    public class ProductCharacteristicViewDto
    {
        public string Name { get; set; } = null!;
        public string? Unit { get; set; }
        public string? Value { get; set; }
    }
}
