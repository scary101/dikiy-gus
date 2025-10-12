using gus_API.Models;
using gus_API.Models.DTOs.ProductDTOs;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace gus_API.Service
{
    public class CharacteristicService
    {
        private readonly AppDbContext _context;

        public CharacteristicService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddCharacter(CharacteristicDto model)
        {

            if(model == null)
            {
                throw new InvalidOperationException("Модель не может быть пустой");
            }
            if(_context.Characteristics.Any(i => i.Name == model.Name))
            {
                throw new InvalidOperationException("Такая характеристика уже есть");
            }

            var character = new Characteristic
            {
                Name = model.Name,
                Unit = model.Unit,
            };
            _context.Characteristics.Add(character);
            await _context.SaveChangesAsync();
        }
        public async Task AddDepend(DependCharacterCategoryDto model)
        {
            if(model == null)
            {
                throw new InvalidOperationException("Модель не может быть пустой");
            }
            bool exists = await _context.CategoryCharacteristics
            .AnyAsync(i => i.Categoryid == model.categoryId && i.Characteristicid == model.characterId);

            if(exists)
            {
                throw new InvalidOperationException("Связь уже существует");
            }

            var link = new CategoryCharacteristic
            {
                Characteristicid = model.characterId,
                Categoryid = model.categoryId,
            };
            _context.CategoryCharacteristics.Add(link);
            await _context.SaveChangesAsync();
        }
        public async Task<List<CharacteristicDto>> GetAll()
        {
            return _context.Characteristics.Select(i => new CharacteristicDto
            {
                Name = i.Name,
                Unit = i.Unit
            }).ToList();
        }
        public async Task<List<CharacteristicDto>> GetAllByCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(i => i.Id == id);

            if(category == null)
            {
                throw new InvalidOperationException("Категория не найдена");
            }
            return await _context.CategoryCharacteristics
                .Where(i => i.Categoryid == id)
                .Include(i => i.Characteristic)
                .Select(i => new CharacteristicDto
                {
                    Name = i.Characteristic.Name,
                    Unit = i.Characteristic.Unit
                })
                .ToListAsync();
        }
    }
}
