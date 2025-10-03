using System.ComponentModel.DataAnnotations;

namespace gus_API.Models.DTOs
{
    public class EmailDto
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        public string Email { get; set; }
    }
}
