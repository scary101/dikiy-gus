using System.ComponentModel.DataAnnotations;

namespace gus_API.Models.DTOs
{
    public class UserDetailsDto
    {
        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Фамилия должна быть от 1 до 50 символов")]
        [RegularExpression(@"^[\p{L} \-']+$", ErrorMessage = "Фамилия может содержать только буквы")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Имя должно быть от 1 до 50 символов")]
        [RegularExpression(@"^[\p{L} \-']+$", ErrorMessage = "Имя может содержать только буквы")]
        public string? FirstName { get; set; }

        [StringLength(50, MinimumLength = 1, ErrorMessage = "Отчество должно быть от 1 до 50 символов")]
        [RegularExpression(@"^[\p{L} \-']*$", ErrorMessage = "Отчество может содержать только буквы")]
        public string? MiddleName { get; set; }
    }
}
