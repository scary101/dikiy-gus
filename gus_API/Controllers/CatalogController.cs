using gus_API.Models.DTOs.ProductDTOs.ProductCardDTOs;
using gus_API.Service;
using Microsoft.AspNetCore.Mvc;

namespace gus_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogService _catalogService;

        public CatalogController(CatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var cards = await _catalogService.GetAllCards();
                return Ok(cards);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Произошла ошибка на сервере", detail = ex.Message });
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            try
            {
                var cards = await _catalogService.GetByCategory(categoryId);
                return Ok(cards);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Произошла ошибка на сервере", detail = ex.Message });
            }
        }

        [HttpGet("ep/{id}")]
        public async Task<IActionResult> GetByEntrepreneur(int id)
        {
            try
            {
                var cards = await _catalogService.GetByEp(id);
                return Ok(cards);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Произошла ошибка на сервере", detail = ex.Message });
            }
        }

    }
}
