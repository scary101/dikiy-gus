using gus_API.Models;
using gus_API.Models.DTOs;
using gus_API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace gus_API.Service
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly TokenService _tokenService;

        public AuthService(AppDbContext context, EmailService emailService, TokenService tokenService)
        {
            _context = context;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        public async Task RegisterUserAsync(RegisterDto model)
        {
            if (model.Password != model.Confirm)
                throw new InvalidOperationException("Пароли не совпадают");

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                throw new InvalidOperationException("Пользователь с таким email уже существует");

            var user = new User
            {
                Email = model.Email,
                Password = PasswordHasher.HashPassword(model.Password, out string salt),
                RoleId = (int)RoleEnum.Client,
                Salt = salt,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task SendLoginCodeAsync(LoginDto model)
        {
            var user = await _context.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            if (!PasswordHasher.VerifyPassword(model.Password, user.Password, user.Salt))
            {
                user.Attempt++;
                if (user.Attempt > 5)
                {
                    BlockService.BanUserHightAttempt(user);
                }
                await _context.SaveChangesAsync();
                throw new InvalidOperationException("Неверный логин или пароль");
            }

            var code = _emailService.GenerateLoginCode(user);
            await _emailService.SendEmailAsync(user.Email, "Код подтверждения", code);
        }

        public async Task<string> VerifyCodeAsync(VerifyCodeDto model)
        {
            var code = await _context.ConfirmationCodes
                .Include(c => c.User)
                    .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(c => c.User.Email == model.Email && c.Code == model.Code);

            if (code == null)
                throw new InvalidOperationException("Неверный код");

            if (code.ExpiresAt <= DateTime.UtcNow)
                throw new InvalidOperationException("Код подтверждения истек");

            var token = _tokenService.GenerateToken(code.User);

            _context.ConfirmationCodes.Remove(code);
            await _context.SaveChangesAsync();

            return token;
        }

        public async Task SendResetLinkAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return; 

            var userBans = await _context.UserBans.Where(i => i.User == user).ToListAsync();
            if (userBans.Any(b => b.EndDate > DateTime.UtcNow))
                throw new InvalidOperationException("Пользователь заблокирован");

            var token = _tokenService.GeneratePasswordResetToken();

            _context.PasswordResets.Add(new PasswordReset
            {
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                UserId = user.Id,
                Used = false
            });

            await _context.SaveChangesAsync();
            await _emailService.SendPasswordResetEmailAsync(email, token);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto model)
        {
            var tokenRecord = await _context.PasswordResets
                .FirstOrDefaultAsync(t => t.Token == model.Token && !t.Used);

            if (tokenRecord == null || tokenRecord.ExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("Ссылка недействительна или истекла");

            if (model.Password != model.Reset)
                throw new InvalidOperationException("Пароли не совпадают");

            var user = await _context.Users.FindAsync(tokenRecord.UserId);
            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            user.Password = PasswordHasher.HashPassword(model.Password, out string salt);
            user.Salt = salt;

            tokenRecord.Used = true;
            await _context.SaveChangesAsync();
        }
    }

}
