using gus_API.Models;
using gus_API.Models.DTOs;
using gus_API.Models.Enums;
using gus_API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gus_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly JwtService _jwtService;


        public AuthController(AppDbContext context, EmailService emailService, JwtService jwtService)
        {
            _context = context;
            _emailService = emailService;
            _jwtService = jwtService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto model)
        {
            if (model.Password != model.Confirm)
            {
                return BadRequest("Пароли не совпадают!");
            }
            if (_context.Users.FirstOrDefault(i => i.Email == model.Email) != null)
            {
                return BadRequest("Пользователь с таким логином уже существует!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Email = model.Email,
                Password = PasswordHasher.HashPassword(model.Password, out string salt),
                RoleId = (int)RoleEnum.Client,
                CreatedAt = DateTime.Now,
                Salt = salt
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto model)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                return BadRequest("Пользователь не найден");
            }
            if (!PasswordHasher.VerifyPassword(model.Password, user.Password, user.Salt))
            {
                if (user.Attempt > 5)
                {
                    BlockService.BanUserHightAttempt(user);
                    await _context.SaveChangesAsync();
                    return BadRequest("Превышен лимит попыток! Вы были заблокированы, попробуйте позже");
                }
                else
                {
                    user.Attempt++;
                    await _context.SaveChangesAsync();
                    return BadRequest("Неверный логин или пароль!");
                }

            }
            var code = _emailService.GenerateLoginCode(user);

            await _emailService.SendEmailAsync(user.Email, "Код подтверждения", code);
            return Ok();

        }

        [HttpPost("verifycode")]
        public IActionResult VerifyCode([FromBody] VerifyCodeDto model)
        {
            var code = _context.ConfirmationCodes
             .Include(c => c.User)
                 .ThenInclude(u => u.Role)
             .FirstOrDefault(i => i.User.Email == model.Email && i.Code == model.Code);

            if (code == null)
            {
                return BadRequest("Неверный код!");
            }
            if(code.ExpiresAt <=  DateTime.Now)
            {
                return BadRequest("Код подтверждения истек!");
            }

            var user = code.User;

            var token = _jwtService.GenerateToken(user);

            _context.ConfirmationCodes.Remove(code);
            _context.SaveChanges();

            return Ok(new {token});
        }
    }
}
