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
        [HttpGet("main")]
        public async Task<IActionResult> GetMainPageCards([FromQuery] int count = 12)
        {
            var products = await _catalogService.GetMainPageCards(count);
            return Ok(products);
        }
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(
            int categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? filter = null,
            [FromQuery] string sortBy = "id")
        {
            var products = await _catalogService.GetByCategory(categoryId, page, pageSize, filter, sortBy);
            return Ok(products);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetByQuery(
            [FromQuery] string query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string sortBy = "id")
        {
            var products = await _catalogService.GetByQuery(query, page, pageSize, sortBy);
            return Ok(products);
        }

        [HttpGet("entrepreneur/{epId}")]
        public async Task<IActionResult> GetByEp(int epId)
        {
            var products = await _catalogService.GetByEp(epId);
            return Ok(products);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductDetails(int productId)
        {
            var product = await _catalogService.GetProductDetailsAsync(productId);
            return Ok(product);
        }
    }
}
