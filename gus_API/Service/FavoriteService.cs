using gus_API.Models;
using gus_API.Models.DTOs.ProductDTOs.ProductCardDTOs;
using Microsoft.EntityFrameworkCore;

namespace gus_API.Service
{
    public class FavoriteService
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;

        public FavoriteService(AppDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task ToggleFavorite(int productId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
                throw new InvalidOperationException("Товар не найден");
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.ProductId == productId && f.UserId == user.Id);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
            }
            else
            {
                favorite = new Favorite
                {
                    ProductId = productId,
                    UserId = user.Id
                };
                _context.Favorites.Add(favorite);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductCardDto>> GetFavorites()
        {
            var user = await _userService.GetCurrentUserAsync();

            var favoriteProductIds = await _context.Favorites
                .Where(f => f.UserId == user.Id)
                .Select(f => f.ProductId)
                .ToListAsync();

            var products = await _context.Products
                .Where(p => favoriteProductIds.Contains(p.Id))
                .ToListAsync();
            var cards = products.Select(card => new ProductCardDto
            {
                Id = card.Id,
                Name = card.Name,
                Price = card.Price,
                Stock = card.Stock,
                Rating = card.Rating,
                ReviewsCount = card.ReviewsCount
            }).ToList();

            return cards;
        }
        public async Task ClearFavorites()
        {
            var user = await _userService.GetCurrentUserAsync();

            var favorites = await _context.Favorites
                .Where(f => f.UserId == user.Id)
                .ToListAsync();

            if (favorites.Any())
            {
                _context.Favorites.RemoveRange(favorites);
                await _context.SaveChangesAsync();
            }
        }


    }
}
