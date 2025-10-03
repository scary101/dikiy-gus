using System.ComponentModel.DataAnnotations;

namespace gus_API.Models.DTOs
{
    public class ResetPasswordDto
    {
        public string Token {  get; set; }
        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 30 символов")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Пароль должен содержать только латинские буквы и цифры")]
        public string Password { get; set; }
        public string Reset {  get; set; }
    }
}
