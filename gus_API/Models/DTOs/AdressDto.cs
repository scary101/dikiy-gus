using System.ComponentModel.DataAnnotations;

namespace gus_API.Models.DTOs
{
    public class AdressCreateDto
    {
        [Required(ErrorMessage = "Город не может быть пустым")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Не верный ввод")]
        [RegularExpression(@"^[A-Za-zА-Яа-яЁё\s\-]+$", ErrorMessage = "Не верный ввод")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Улица не может быть пустой")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Не верный ввод")]
        public string? Street { get; set; }

        [Required(ErrorMessage = "Дом не может быть пустым")]
        [StringLength(10, ErrorMessage = "Номер дома слишком длинный")]
        [RegularExpression(@"^[0-9A-Za-z\-]+$", ErrorMessage = "Не верный ввод")]
        public string? House { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Не верный ввод")]
        public string? Apartment { get; set; }
    }
    public class AdressUpdateDto : AdressCreateDto
    {
        [Required]
        public int Id { get; set; }
    }
}
