using gus_API.Models;
using gus_API.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace gus_API.Service
{
    public class ProfileService
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;

        public ProfileService(AppDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<UserDetailsDto> GetProfileAsync()
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user == null) throw new KeyNotFoundException("Пользователь не найден");

            var addresses = await _context.Addresses
                .Where(a => a.UserId == user.Id)
                .ToListAsync();

            return new UserDetailsDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MiddleName = user.MiddleName,
                Adress = addresses.Select(a => new AdressUpdateDto
                {
                    Id = a.Id,
                    City = a.City,
                    Street = a.Street,
                    House = a.House,
                    Apartment = a.Apartment
                }).ToList()
            };
        }

        public async Task UpdateUserDetailsAsync(UserDetailsDto model)
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user == null) throw new KeyNotFoundException("Пользователь не найден");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.MiddleName = model.MiddleName;

            await _context.SaveChangesAsync();
        }

        public async Task<Address> AddAddressAsync(AdressCreateDto model)
        {
            var user = await _userService.GetCurrentUserAsync();

            var address = new Address
            {
                UserId = user.Id,
                City = model.City,
                Street = model.Street,
                House = model.House,
                Apartment = model.Apartment
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address> UpdateAddressAsync(int addressId, AdressUpdateDto model)
        {
            var userId = _userService.GetCurrentUserId();
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
            if (address == null) throw new KeyNotFoundException("Адрес не найден");

            address.City = model.City;
            address.Street = model.Street;
            address.House = model.House;
            address.Apartment = model.Apartment;

            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address> GetAddressAsync(int addressId)
        {
            var userId = _userService.GetCurrentUserId();
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
            if (address == null) throw new KeyNotFoundException("Адрес не найден");
            return address;
        }
    }

}
