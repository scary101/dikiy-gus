using System.ComponentModel.DataAnnotations;

namespace gus_API.Models.DTOs.SupllyDTOs
{
    public class SupplyCreateDto
    {
        [MinLength(1, ErrorMessage = "Поставка должна содержать хотя бы один товар.")]
        public List<SupplyItemCreateDto> Items { get; set; } = new();
    }

    public class SupplyItemCreateDto
    {
        [Required(ErrorMessage = "Необходимо указать ID товара.")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0.")]
        public int Quantity { get; set; }
    }

}
