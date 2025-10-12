using System.ComponentModel.DataAnnotations;

namespace gus_API.Models.DTOs.CategoryDTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Не верный ввод")]
        public string Name { get; set; } = null!;
        [RegularExpression(@"^[0-9]\d*$", ErrorMessage = "Не верный ввод")]
        public int? ParentId { get; set; }

    }
    public class CreateCategoryDto
    {
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Не верный ввод")]
        public string Name { get; set; } = null!;
        [RegularExpression(@"^[0-9]\d*$", ErrorMessage = "Не верный ввод")]
        public int? ParentId { get; set; }
    }
}
