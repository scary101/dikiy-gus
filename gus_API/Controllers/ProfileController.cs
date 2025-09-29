using gus_API.Models;
using gus_API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace gus_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid token");
            }

            var user = await _context.Users.FirstOrDefaultAsync(i => i.Id == userId);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            var addresses = await _context.Addresses
                .Where(i => i.UserId == userId)
                .ToListAsync();

            var allAdress = addresses.Select(address => new AdressUpdateDto
            {
                Id = address.Id,
                City = address.City,
                Street = address.Street,
                House = address.House,
                Apartment = address.Apartment
            }).ToList();

            var details = new UserDetailsDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MiddleName = user.MiddleName,
                Adress = allAdress
            };

            return Ok(details);
        }


        [HttpPut("upsertDetails")]
        public async Task<IActionResult> AddDetails([FromBody] UserDetailsDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid token");
            }
            var user = _context.Users.FirstOrDefault(i => i.Id == userId);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.MiddleName = model.MiddleName;

            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("addAdress")]
        public async Task<IActionResult> AddAdress([FromBody] AdressCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid token");
            }

            var user = _context.Users.FirstOrDefault(i =>i.Id == userId);

            var adress = new Address
            {
                UserId = user.Id,
                City = model.City,
                Street = model.Street,
                House = model.House,
                Apartment = model.Apartment
            };

            _context.Addresses.Add(adress);
            await _context.SaveChangesAsync();
            return Ok(adress);
        }

        [HttpPut("changeAdress/{addressId}")]
        public async Task<IActionResult> ChangeAdress([FromBody] AdressUpdateDto model, int addressId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid token");

            var adress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (adress == null)
                return NotFound("Адрес не найден");

            adress.City = model.City;
            adress.Street = model.Street;
            adress.House = model.House;
            adress.Apartment = model.Apartment;

            await _context.SaveChangesAsync();
            return Ok(adress);
        }
        [HttpGet("getAdress/{adressId}")]
        public async Task<IActionResult> GetAdress(int adressId)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid token");

            var adress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == adressId && a.UserId == userId);

            if (adress == null)
                return NotFound("Адрес не найден");

            return Ok(adress);
        } 
    }
}
