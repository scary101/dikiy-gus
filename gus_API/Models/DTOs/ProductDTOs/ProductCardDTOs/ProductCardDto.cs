namespace gus_API.Models.DTOs.ProductDTOs.ProductCardDTOs
{
    public class ProductCardDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public int? Stock { get; set; }

        public decimal? Rating { get; set; }

        public int? ReviewsCount { get; set; }

        public string? PhotoPath { get; set; }
        public bool IsFavorite { get; set; }

    }
}
