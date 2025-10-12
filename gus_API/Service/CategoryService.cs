using gus_API.Models;
using gus_API.Models.DTOs.CategoryDTOs;
using Microsoft.EntityFrameworkCore;

namespace gus_API.Service
{
    public class CategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddCategory(CreateCategoryDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.ParentId.HasValue && model.ParentId.Value != 0)
            {
                var parent = await _context.Categories
                    .FirstOrDefaultAsync(i => i.Id == model.ParentId.Value);

                if (parent == null)
                { 
                    throw new ArgumentException("Родительская категория не найдена");
                }
            }

            int? parentIdToSave = model.ParentId == 0 ? null : model.ParentId;

            var category = new Category
            {
                Name = model.Name,
                ParentId = parentIdToSave
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }


        public async Task<List<CategoryDto>> GetCategories()
        {
            var categories = await _context.Categories
                .Where(i => i.ParentId == null)
                .ToListAsync();

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentId
            }).ToList();
        }

        public async Task<List<CategoryDto>> GetSubCategories(int id)
        {
            var categories = await _context.Categories
                .Where(i => i.ParentId == id)
                .ToListAsync();

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentId
            }).ToList();
        }
        public async Task<List<CategoryDto>> GetAll()
        {
            return _context.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentId
            }).ToList();
        }

        public async Task RenameCategory(CategoryDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var category = await _context.Categories
                .FirstOrDefaultAsync(i => i.Id == model.Id);

            if (category == null)
                throw new InvalidOperationException("Категория не найдена");

            bool hasChildren = await _context.Categories
                .AnyAsync(i => i.ParentId == category.Id);

            if (hasChildren && category.ParentId != model.ParentId)
            {
                throw new InvalidOperationException(
                    "Нельзя изменить родительскую категорию, пока у неё есть дочерние категории.");
            }

            if (model.ParentId.HasValue && model.ParentId.Value != 0)
            {
                var parent = await _context.Categories
                    .FirstOrDefaultAsync(i => i.Id == model.ParentId.Value);

                if (parent == null)
                    throw new ArgumentException("Родительская категория не найдена");
            }

            category.ParentId = model.ParentId == 0 ? null : model.ParentId;

            category.Name = model.Name;

            await _context.SaveChangesAsync();
        }

    }
}
