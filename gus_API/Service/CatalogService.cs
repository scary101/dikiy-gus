using gus_API.Models;
using gus_API.Models.DTOs.ProductDTOs;
using gus_API.Models.DTOs.ProductDTOs.ProductCardDTOs;
using Microsoft.EntityFrameworkCore;

namespace gus_API.Service
{
    public class CatalogService
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;
        public CatalogService(AppDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }
        private async Task<List<ProductCardDto>> GetCards(List<Product> products)
        {
            var user = await _userService.GetCurrentUserAsync();

            List<int> favoriteProductIds = new();

            if (user != null)
            {
                favoriteProductIds = await _context.Favorites
                    .Where(f => f.UserId == user.Id)
                    .Select(f => f.ProductId)
                    .ToListAsync();
            }

            var cards = new List<ProductCardDto>();

            foreach (var card in products)
            {
                cards.Add(new ProductCardDto
                {
                    Id = card.Id,
                    Name = card.Name,
                    Price = card.Price,
                    Stock = card.Stock,
                    Rating = card.Rating,
                    ReviewsCount = card.ReviewsCount,
                    PhotoPath = card.PhotoPath,
                    IsFavorite = user != null && favoriteProductIds.Contains(card.Id)
                });
            }

            return cards;
        }



        private IQueryable<Product> ApplyFilterAndSort(
            IQueryable<Product> query,
            string? filter = null,
            string sortBy = "id")
        {
            if (!string.IsNullOrEmpty(filter))
                query = query.Where(p => EF.Functions.Like(p.Name, $"%{filter}%"));

            query = sortBy.ToLower() switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "rating_asc" => query.OrderBy(p => p.Rating),
                "rating_desc" => query.OrderByDescending(p => p.Rating),
                _ => query.OrderBy(p => p.Id)
            };

            return query;
        }

        public async Task<List<ProductCardDto>> GetMainPageCards(int count = 12)
        {
            var products = await _context.Products
                .Where(p => p.IsActive == true && p.Stock > 0)
                .OrderBy(p => Guid.NewGuid())
                .Take(count)
                .ToListAsync();

            if (!products.Any())
                throw new InvalidOperationException("Товары не найдены");

            return await GetCards(products);
        }

        public async Task<List<ProductCardDto>> GetByCategory(
            int categoryId,
            int page = 1,
            int pageSize = 20,
            string? filter = null,
            string sortBy = "id")
        {
            var categoryIds = await _context.Categories
                .Where(c => c.Id == categoryId || c.ParentId == categoryId)
                .Select(c => c.Id)
                .ToListAsync();

            var query = _context.Products
                .Where(p => p.IsActive == true && p.Stock > 0 && p.CategoryId.HasValue && categoryIds.Contains(p.CategoryId.Value));

            query = ApplyFilterAndSort(query, filter, sortBy);

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!products.Any())
                throw new InvalidOperationException("Товары не найдены");

            return await GetCards(products);
        }

        public async Task<List<ProductCardDto>> GetByQuery(
            string query,
            int page = 1,
            int pageSize = 20,
            string sortBy = "id")
        {
            var q = _context.Products
                .Where(p => p.IsActive == true && EF.Functions.Like(p.Name, $"%{query}%"));

            q = ApplyFilterAndSort(q, null, sortBy);

            var products = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!products.Any())
                throw new InvalidOperationException("Ничего не найдено");

            return await GetCards(products);
        }

        public async Task<ProductCardEpDto> GetByEp(int id)
        {
            var product = await _context.Products
                .Where(p => p.IsActive == true && p.Stock > 0 && p.EntrepreneurId == id)
                .ToListAsync();

            var ep = await _context.Entrepreneurs.FirstOrDefaultAsync(i => i.Id == id);

            if (product == null || ep == null)
                throw new InvalidOperationException("Товары не найдены");

            var cards = await GetCards(product);

            return new ProductCardEpDto
            {
                products = cards,
                MagazineName = ep.MagazinName,
                EpName = ep.ShortName
            };
        }
        public async Task<ProductDetailDto> GetProductDetailsAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Entrepreneur)
                .Include(p => p.ProductCharacteristics)
                    .ThenInclude(pc => pc.Characteristic)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive == true);

            if (product == null)
                throw new InvalidOperationException("Товар не найден");

            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                IsActive = product.IsActive,
                Rating = product.Rating ?? 0,
                ReviewsCount = product.ReviewsCount ?? 0,
                PhotoPath = product.PhotoPath,
                CreatedAt = product.CreatedAt,

                Category = product.Category == null ? null : new ProductDetailCategoryDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    ParentId = product.Category.ParentId
                },

                Entrepreneur = new ProductDetailEntrepreneurDto
                {
                    Id = product.Entrepreneur.Id,
                    MagazinName = product.Entrepreneur.MagazinName
                },

                Characteristics = product.ProductCharacteristics.Select(pc => new ProductDetailCharacteristicDto
                {
                    Id = pc.CharacteristicId,
                    Name = pc.Characteristic.Name,
                    Unit = pc.Characteristic.Unit,
                    Value = pc.Value
                }).ToList(),

                Reviews = product.Reviews.Select(r => new ProductDetailReviewDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Text = r.Text,
                    CreatedAt = r.CreatedAt
                }).ToList()
            };
        }
    }
}
