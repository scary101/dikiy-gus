using System.ComponentModel.DataAnnotations;

namespace gus_API.Models.DTOs.AdminDTOs
{
    public class AdminRegisterDto
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 30 символов")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Пароль должен содержать только латинские буквы и цифры")]
        public string Password { get; set; }

        public int RoleId {  get; set; }
        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public string? MiddleName { get; set; }

    }
}
