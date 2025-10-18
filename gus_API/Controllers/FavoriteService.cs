using gus_API.Models.DTOs.ProductDTOs.ProductCardDTOs;
using gus_API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gus_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly FavoriteService _favoriteService;

        public FavoritesController(FavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost("toggle/{productId}")]
        public async Task<IActionResult> ToggleFavorite(int productId)
        {
            await _favoriteService.ToggleFavorite(productId);
            return Ok(new { message = "Избранное обновлено" });
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductCardDto>>> GetFavorites()
        {
            var favorites = await _favoriteService.GetFavorites();
            return Ok(favorites);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearFavorites()
        {
            await _favoriteService.ClearFavorites();
            return Ok(new { message = "Избранное очищено" });
        }
    }
}
