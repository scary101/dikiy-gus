using gus_API.Models.DTOs.ProductDTOs;
using gus_API.Service;
using Microsoft.AspNetCore.Mvc;

namespace gus_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromForm] ProductCreateDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var productId = await _service.AddProduct(model);
                return Ok(new
                {
                    message = "Товар успешно добавлен",
                    id = productId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Ошибка при добавлении товара" });
            }
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutProduct([FromForm] ProductCreateDto model, int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                await _service.UpdateProduct(id, model);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Ошибка при изменении товара" });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _service.GetAllProducts();
                return Ok(products);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Ошибка при получении списка товаров" });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var products = await _service.GetAllProducts();
                var product = products.FirstOrDefault(p => p.Id == id);

                if (product == null)
                    return NotFound(new { error = "Товар не найден" });

                return Ok(product);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Ошибка при получении товара" });
            }
        }
        [HttpGet("byep")]
        public async Task<IActionResult> GetProductsByEp()
        {
            try
            {
                var products = await _service.GetProductsByEp();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}
