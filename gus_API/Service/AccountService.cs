using gus_API.Models;
using gus_API.Models.DTOs.AccountDTOs;
using gus_API.Models.DTOs.AdminDTOs;
using Microsoft.EntityFrameworkCore;

namespace gus_API.Service
{
    public class AccountService
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public AccountService(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public async Task<List<AccountInfoDto>> GetAll()
        {
            var accounts = await _context.Users
                .Include(i => i.Role)
                .ToListAsync();

            return accounts.Select(i => new AccountInfoDto
            {
                Id = i.Id,
                Email = i.Email,
                Role = i.Role.Name,
                IsActive = i.IsActive,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                Isep = i.Isep
            }).ToList();
        }
        public async Task CreateNewUser(AdminRegisterDto model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                throw new InvalidOperationException("Пользователь с таким email уже существует");

            var user = new User
            {
                Email = model.Email,
                Password = PasswordHasher.HashPassword(model.Password, out string salt),
                Salt = salt,
                RoleId = model.RoleId,
                LastName = model.LastName,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
            };
            var info = new InfoRegEmailDto
            {
                Email = user.Email,
                Password = model.Password,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName
            };
            await _emailService.SendEpmlRegisterInfo(info);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task BanUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user.IsActive)
            {
                user.IsActive = false;
            }
            else
            {
                user.IsActive = true;
            }

            await _emailService.SendEmailBanEntry(user.Email, user.IsActive);
            await _context.SaveChangesAsync();
        }
    }
}
