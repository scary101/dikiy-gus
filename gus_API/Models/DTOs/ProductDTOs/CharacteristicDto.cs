using System.ComponentModel.DataAnnotations;

namespace gus_API.Models.DTOs.ProductDTOs
{
    public class CharacteristicDto
    {
        [Required(ErrorMessage = "Поле 'Name' обязательно для заполнения")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Длина названия должна быть от 2 до 100 символов")]
        public string Name { get; set; } = null!;

        [StringLength(20, ErrorMessage = "Единица измерения не может превышать 20 символов")]
        public string? Unit { get; set; }
    }
    public class DependCharacterCategoryDto
    {
        [Required(ErrorMessage = "Укажите ID категории")]
        [Range(1, int.MaxValue, ErrorMessage = "ID категории должен быть положительным числом")]
        public int categoryId { get; set; }

        [Required(ErrorMessage = "Укажите ID характеристики")]
        [Range(1, int.MaxValue, ErrorMessage = "ID характеристики должен быть положительным числом")]
        public int characterId { get; set; }
    }
}

