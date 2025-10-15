using gus_API.Models;
using gus_API.Models.DTOs.ProductDTOs;
using gus_API.Models.DTOs.ProductDTOs.ProductCardDTOs;
using Microsoft.EntityFrameworkCore;

namespace gus_API.Service
{
    public class CatalogService
    {
        private readonly AppDbContext _context;
        public CatalogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductCardDto>> GetCards(List<Product> product)
        {
            var cards = new List<ProductCardDto>();
            foreach (var card in product)
            {
                cards.Add(new ProductCardDto
                {
                    Id = card.Id,
                    Name = card.Name,
                    Price = card.Price,
                    Stock = card.Stock,
                    Rating = card.Rating,
                    ReviewsCount = card.ReviewsCount,
                });
            }
            return cards;
        }

        public async Task<List<ProductCardDto>> GetAllCards()
        {
            var product = _context.Products.Where(i => i.IsActive == true && i.Stock > 0).ToList();
            
            if(product == null)
            {
                throw new InvalidOperationException("Товары не найдены");
            }
            var cards = await GetCards(product);
            return cards;
        }
        public async Task<List<ProductCardDto>> GetByCategory(int categoryId)
        {
            var categoryIds = await _context.Categories
                .Where(c => c.Id == categoryId || c.ParentId == categoryId)
                .Select(c => c.Id)
                .ToListAsync();

            var products = await _context.Products
                .Where(p => p.IsActive == true && p.Stock > 0 && p.CategoryId.HasValue && categoryIds.Contains(p.CategoryId.Value))
                .ToListAsync();

            if (!products.Any())
                throw new InvalidOperationException("Товары не найдены");

            var cards = await GetCards(products);
            return cards;
        }
        public async Task<List<ProductCardDto>> GetByEp(int id)
        {
            var product = _context.Products.Where(i => i.IsActive == true && i.Stock > 0 && i.EntrepreneurId == id).ToList();
            if (product == null)
            {
                throw new InvalidOperationException("Товары не найдены");
            }
            var cards = await GetCards(product);
            return cards;

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
