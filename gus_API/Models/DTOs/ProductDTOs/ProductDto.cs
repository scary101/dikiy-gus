using System.ComponentModel.DataAnnotations;

namespace gus_API.Models.DTOs.ProductDTOs
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Категория обязательна.")]
        [Range(1, int.MaxValue, ErrorMessage = "Некорректный идентификатор категории.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Название товара обязательно.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Название должно быть от 2 до 100 символов.")]
        public string Name { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Описание не может превышать 1000 символов.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Цена обязательна.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0.")]
        public decimal Price { get; set; }

        public bool? IsActive { get; set; } = false;

        public IFormFile? Photo { get; set; }

        [MinLength(1, ErrorMessage = "Необходимо указать хотя бы одну характеристику.")]
        public List<ProductCharacteristicCreateDto>? Characteristics { get; set; }
    }
    public class ProductListDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int? Stock { get; set; }

        public bool? IsActive { get; set; }

        public string? CategoryName { get; set; }

        public string? PhotoPath { get; set; }

        public decimal? Rating { get; set; }

        public List<ProductCharacteristicViewDto>? Characteristics { get; set; }
    }
}
