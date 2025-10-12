using gus_API.Models;
using gus_API.Models.DTOs.ProductDTOs;
using Microsoft.EntityFrameworkCore;

namespace gus_API.Service
{
    public class ProductService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserService _userService;

        public ProductService(AppDbContext context, IWebHostEnvironment env, UserService userService)
        {
            _context = context;
            _env = env;
            _userService = userService;
        }

        public async Task<int> AddProduct(ProductCreateDto model)
        {
            if (model == null)
                throw new InvalidOperationException("Модель не может быть пустой.");

            var user = await _userService.GetCurrentEntrepreneurAsync();
            if (user == null)
                throw new InvalidOperationException("Пользователь не найден.");

            var product = new Product
            {
                EntrepreneurId = user.Id,
                CategoryId = model.CategoryId,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                IsActive = model.IsActive ?? true,
                CreatedAt = DateTime.Now
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            if (model.Photo != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Photo.FileName)}";
                string filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Photo.CopyToAsync(stream);
                }

                string relativePath = $"/images/products/{fileName}";
                product.PhotoPath = relativePath;

                await _context.SaveChangesAsync();
            }

            if (model.Characteristics != null && model.Characteristics.Any())
            {
                var productCharacteristics = model.Characteristics.Select(c => new ProductCharacteristic
                {
                    ProductId = product.Id,
                    CharacteristicId = c.CharacteristicId,
                    Value = c.Value
                }).ToList();

                _context.ProductCharacteristics.AddRange(productCharacteristics);
                await _context.SaveChangesAsync();
            }

            return product.Id;
        }

        public async Task<List<ProductListDto>> GetAllProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductCharacteristics)
                    .ThenInclude(pc => pc.Characteristic)
                .ToListAsync();

            return products.Select(p => new ProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                IsActive = p.IsActive,
                CategoryName = p.Category?.Name,
                PhotoPath = p.PhotoPath,
                Rating = p.Rating,
                Characteristics = p.ProductCharacteristics.Select(pc => new ProductCharacteristicViewDto
                {
                    Name = pc.Characteristic.Name,
                    Unit = pc.Characteristic.Unit,
                    Value = pc.Value
                }).ToList()
            }).ToList();
        }
        public async Task<List<ProductListDto>> GetProductsByEp()
        {
            var user = await _userService.GetCurrentEntrepreneurAsync();
            if (user == null)
                throw new InvalidOperationException("Пользователь не найден.");
            var products = await _context.Products.Where(i => i.EntrepreneurId == user.Id)
                .Include(p => p.Category)
                .Include(p => p.ProductCharacteristics)
                    .ThenInclude(pc => pc.Characteristic)
                .ToListAsync();

            return products.Select(p => new ProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                IsActive = p.IsActive,
                CategoryName = p.Category?.Name,
                PhotoPath = p.PhotoPath,
                Rating = p.Rating,
                Characteristics = p.ProductCharacteristics.Select(pc => new ProductCharacteristicViewDto
                {
                    Name = pc.Characteristic.Name,
                    Unit = pc.Characteristic.Unit,
                    Value = pc.Value
                }).ToList()
            }).ToList();
        }
        public async Task UpdateProduct(int productId, ProductCreateDto model)
        {
            if (model == null)
                throw new InvalidOperationException("Модель не может быть пустой.");

            var user = await _userService.GetCurrentEntrepreneurAsync();
            if (user == null)
                throw new InvalidOperationException("Пользователь не найден.");

            var product = await _context.Products
                .Include(p => p.ProductCharacteristics)
                .FirstOrDefaultAsync(p => p.Id == productId && p.EntrepreneurId == user.Id);

            if (product == null)
                throw new InvalidOperationException("Продукт не найден или не принадлежит текущему предпринимателю.");

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.IsActive = model.IsActive ?? true;
            product.CategoryId = model.CategoryId;
            product.UpdatedAt = DateTime.Now;

            if (model.Photo != null)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                if (!string.IsNullOrEmpty(product.PhotoPath))
                {
                    string oldPath = Path.Combine(_env.WebRootPath, product.PhotoPath.TrimStart('/'));
                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
                }

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Photo.FileName)}";
                string filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Photo.CopyToAsync(stream);
                }

                product.PhotoPath = $"/images/products/{fileName}";
            }

            if (model.Characteristics != null)
            {
                _context.ProductCharacteristics.RemoveRange(product.ProductCharacteristics);

                var newCharacteristics = model.Characteristics.Select(c => new ProductCharacteristic
                {
                    ProductId = product.Id,
                    CharacteristicId = c.CharacteristicId,
                    Value = c.Value
                }).ToList();

                _context.ProductCharacteristics.AddRange(newCharacteristics);
            }

            await _context.SaveChangesAsync();
        }

    }
}
