using System.ComponentModel.DataAnnotations;

namespace gus_API.Models.DTOs
{
    public class EntrepreneurDto
    {
        [Required]
        [RegularExpression(@"^\d{20}$", ErrorMessage = "Номер расчетного счета должен содержать 20 цифр.")]
        public string? AccountNumber { get; set; }

        [Required]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "ИНН должен содержать 12 цифр.")]
        public string Inn { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "Краткое наименование не должно превышать 100 символов.")]
        public string ShortName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "БИК должен содержать 9 цифр.")]
        public string Bik { get; set; } = null!;

        [Required]
        [StringLength(200, ErrorMessage = "Полное наименование не должно превышать 200 символов.")]
        public string FullName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{15}$", ErrorMessage = "ОГРНИП должен содержать 15 цифр.")]
        public string Ogrnip { get; set; } = null!;

        [Required]
        [StringLength(300, ErrorMessage = "Юридический адрес не должен превышать 300 символов.")]
        public string LegalAddress { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "Название магазина не должно превышать 100 символов.")]
        public string MagazinName { get; set; } = null!;
    }
}
