using gus_API.DTOs;
using gus_API.Models;
using gus_API.Models.DTOs.SupllyDTOs;
using gus_API.Models.DTOs.SupplyDTOs;
using gus_API.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace gus_API.Service
{
    public class SupplyService
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;
        public SupplyService(AppDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task CreateSupplyAsync(SupplyCreateDto model)
        {
            if (model == null || model.Items == null || !model.Items.Any())
                throw new InvalidOperationException("Список товаров не может быть пустым.");

            var ep = await _userService.GetCurrentEntrepreneurAsync();
            if (ep == null)
                throw new InvalidOperationException("Предприниматель не найден.");

            var supply = new Supply
            {
                EntrepreneurId = ep.Id,
                StatusId = 1,
                CreatedAt = DateTime.Now,
                SupplyItems = new List<SupplyItem>()
            };

            foreach (var item in model.Items)
            {
                supply.SupplyItems.Add(new SupplyItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            _context.Supplies.Add(supply);
            await _context.SaveChangesAsync();
        }
        public async Task PutStatus(SupplyStatusDto model)
        {
            var manager = await _userService.GetCurrentManagerId();

            var supply = await _context.Supplies
                .Include(s => s.SupplyItems)
                .FirstOrDefaultAsync(i => i.Id == model.Supply_id);

            if (supply == null)
                throw new InvalidOperationException("Поставка не найдена");

            supply.StatusId = model.Status_id;

            if (model.Status_id == 2)
            {
                supply.ManagerId = manager;
                supply.CompletedAt = DateTime.Now;
                var productIds = supply.SupplyItems.Select(si => si.ProductId).ToList();

                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();
                foreach (var supplyItem in supply.SupplyItems)
                {
                    var product = products.FirstOrDefault(p => p.Id == supplyItem.ProductId);
                    if (product != null)
                    {
                        product.Stock = (product.Stock ?? 0) + supplyItem.Quantity;
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task<List<SupplyListDto>> GetAllSupplies()
        {
            var supplies = await _context.Supplies
                .Include(s => s.Entrepreneur).ThenInclude(e => e.User)
                .Include(s => s.Status)
                .Include(s => s.Manager)
                .Include(s => s.SupplyItems).ThenInclude(si => si.Product)
                .ToListAsync();

            return supplies.Select(s => new SupplyListDto
            {
                Id = s.Id,
                EntrepreneurName = s.Entrepreneur?.User?.FirstName ?? "Неизвестно",
                Status = s.Status?.Name ?? "Неизвестен",
                CreatedAt = s.CreatedAt,
                ComletedAt = s.CompletedAt,
                Manager = s.Manager == null ? null : new ManagerDto
                {
                    Id = s.Manager.Id,
                    FirstName = s.Manager.FirstName,
                    MiddleName = s.Manager.MiddleName,
                    Email = s.Manager.Email
                },
                Items = s.SupplyItems.Select(i => new SupplyItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "Неизвестный товар",
                    Quantity = i.Quantity
                }).ToList()
            }).ToList();
        }


        public async Task<List<SupplyListDto>> GetSuppliesByEntrepreneur()
        {
            var ep = await _userService.GetCurrentEntrepreneurAsync();
            if (ep == null)
                throw new InvalidOperationException("Предприниматель не найден.");

            var supplies = await _context.Supplies
                .Where(s => s.EntrepreneurId == ep.Id)
                .Include(s => s.Status)
                .Include(s => s.Manager)
                .Include(s => s.SupplyItems).ThenInclude(si => si.Product)
                .ToListAsync();

            return supplies.Select(s => new SupplyListDto
            {
                Id = s.Id,
                EntrepreneurName = $"{ep.User.FirstName} {ep.User.LastName}",
                Status = s.Status?.Name ?? "Неизвестен",
                CreatedAt = s.CreatedAt,
                ComletedAt = s.CreatedAt,
                Manager = s.Manager == null ? null : new ManagerDto
                {
                    Id = s.Manager.Id,
                    FirstName = s.Manager.FirstName,
                    MiddleName = s.Manager.MiddleName,
                    Email = s.Manager.Email
                },
                Items = s.SupplyItems.Select(i => new SupplyItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "Неизвестный товар",
                    Quantity = i.Quantity
                }).ToList()
            }).ToList();
        }
        public async Task<SupplyDocumentDto?> GetSupplyDocumentAsync(int supplyId)
        {
            var supply = await _context.Supplies
                .Include(s => s.Entrepreneur)
                .Include(s => s.Manager)
                    .ThenInclude(m => m.Role)
                .Include(s => s.Status)
                .Include(s => s.SupplyItems)
                    .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(s => s.Id == supplyId);

            if (supply == null) return null;

            return MapToDto(supply);
        }
        public SupplyDocumentDto MapToDto(Supply supply)
        {
            return new SupplyDocumentDto
            {
                SupplyId = supply.Id,
                CreatedAt = supply.CreatedAt,
                Status = supply.Status != null ? supply.Status.GetType().Name : "Неизвестно",

                EntrepreneurFullName = supply.Entrepreneur.FullName,
                EntrepreneurShortName = supply.Entrepreneur.ShortName,
                EntrepreneurInn = supply.Entrepreneur.Inn,
                EntrepreneurOgrnip = supply.Entrepreneur.Ogrnip,
                EntrepreneurLegalAddress = supply.Entrepreneur.LegalAddress,
                EntrepreneurMagazinName = supply.Entrepreneur.MagazinName,

                ManagerFullName = supply.Manager != null
                    ? $"{supply.Manager.FirstName} {supply.Manager.LastName}"
                    : null,
                ManagerEmail = supply.Manager?.Email,
                ManagerRole = supply.Manager?.Role != null ? supply.Manager.Role?.ToString() : null,

                Items = supply.SupplyItems.Select(si => new SupplyItemDtoIn
                {
                    ProductName = si.Product.Name,
                    Quantity = si.Quantity
                }).ToList()
            };
        }



    }
}
