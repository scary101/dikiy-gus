using gus_API.Models;
using gus_API.Models.DTOs;
using gus_API.Models.DTOs.AccountDTOs;
using gus_API.Models.DTOs.AdminDTOs;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

namespace gus_API.Service
{
    public class SellerService
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly UserService _userService;

        public SellerService(AppDbContext context, EmailService emailService, UserService userService)
        {
            _context = context;
            _emailService = emailService;
            _userService = userService;
        }

        public async Task RegisterEP(EntrepreneurDto model)
        {
            if (await _context.Entrepreneurs.AnyAsync(e => e.Inn == model.Inn))
                throw new InvalidOperationException("ИП с таким ИНН уже зарегистрирован.");

            if (await _context.Entrepreneurs.AnyAsync(e => e.Ogrnip == model.Ogrnip))
                throw new InvalidOperationException("ИП с таким ОГРНИП уже зарегистрирован.");

            if (!string.IsNullOrEmpty(model.AccountNumber))
            {
                if (await _context.Entrepreneurs.AnyAsync(e => e.AccountNumber == model.AccountNumber))
                    throw new InvalidOperationException("Такой расчетный счет уже используется.");
                if (await _context.Entrepreneurs.AnyAsync(e => e.MagazinName == model.MagazinName))
                    throw new InvalidOperationException("Название магазина уже используется.");
            }
            var user = await _userService.GetCurrentUserAsync();
            var ep = new Entrepreneur
            {
                UserId = user.Id,
                AccountNumber = model.AccountNumber,
                CreatedAt = DateTime.Now,
                Inn = model.Inn,
                ShortName = model.ShortName,
                Bik = model.Bik,
                FullName = model.FullName,
                Ogrnip = model.Ogrnip,
                LegalAddress = model.LegalAddress,
                MagazinName = model.MagazinName
            };
            _context.Entrepreneurs.Add(ep);
            await _context.SaveChangesAsync();
        }
        public async Task EnterEp(int id)
        {
            var ep = _context.Entrepreneurs.FirstOrDefault(e => e.Id == id);
            var user = await _userService.GetCurrentUserAsync();

            if(ep.IsActive == false)
            {
                ep.IsActive = true;
                user.Isep = true;
            }
            else
            {
                if (_context.Entrepreneurs.Any(i => i.UserId == user.Id))
                {
                    ep.IsActive = false;
                    user.Isep = false;
                }
                else
                {
                    throw new InvalidOperationException("Пользователь не имеет коммерческого аккаунта");
                } 
            }
            await _emailService.SendEmailEpEntry(_context.Users.FirstOrDefault(i => i.Id == ep.UserId).Email, ep.IsActive);
            await _context.SaveChangesAsync();
        }
        public async Task<EntrepreneurDto> GetInfoEp()
        {
            var user = await _userService.GetCurrentUserAsync();
            var ep =  _context.Entrepreneurs.FirstOrDefault(ep => ep.Id == user.Id);

            if(user.IsActive == false || user.Isep ==false)
            {
                throw new InvalidOperationException("У пользователя нет доступа к функциям ИП");
            }

            var epDto = new EntrepreneurDto
            {
                AccountNumber = ep.AccountNumber,
                Inn = ep.Inn,
                ShortName = ep.ShortName,
                Bik = ep.Bik,
                FullName = ep.FullName,
                Ogrnip = ep.Ogrnip,
                LegalAddress = ep.LegalAddress,
                MagazinName = ep.MagazinName
            };
            return epDto;
        }
        public async Task<List<AdminEpDto>> GetAll()
        {
            var ep = _context.Entrepreneurs.ToList();
            return ep.Select(i => new AdminEpDto
            {
                Id = i.Id,
                AccountNumber = i.AccountNumber,
                UserId = i.UserId,
                IsActive = i.IsActive,
                LegalAddress = i.LegalAddress,
                CreatedAt = i.CreatedAt,
            }).ToList();
        }
        public async Task<EpDetailDto> GetInfoById(int id)
        {
            var ep = _context.Entrepreneurs.FirstOrDefault(i => i.Id == id);

            var epdetail = new EpDetailDto
            {
                Id = ep.Id,
                UserId = ep.UserId,
                AccountNumber = ep.AccountNumber,
                WalletId = ep.WalletId,
                IsActive = ep.IsActive,
                CreatedAt = ep.CreatedAt,
                Inn = ep.Inn,
                ShortName = ep.ShortName,
                Bik = ep.Bik,
                FullName = ep.FullName,
                Ogrnip = ep.Ogrnip,
                LegalAddress = ep.LegalAddress,
                MagazinName = ep.MagazinName
            };
            return epdetail;
        }
    }
}
