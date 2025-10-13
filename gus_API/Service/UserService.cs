using gus_API.Models;
using gus_API.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace gus_API.Service
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("Invalid token");
            return userId;
        }
        public async Task<int> GetCurrentManagerId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("Invalid token");
            var user = await _context.Users.FindAsync(userId);

            if (user == null || !user.RoleId.Equals((int)RoleEnum.Manager))
            {
                throw new UnauthorizedAccessException("В доступе отказано");
            }   
            return userId;
        }
        public async Task<Entrepreneur> GetCurrentEntrepreneurAsync()
        {
            int userId = GetCurrentUserId();

            var entrepreneur = await _context.Entrepreneurs
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (entrepreneur == null)
                throw new InvalidOperationException("Предприниматель не найден");

            return entrepreneur;
        }

        public Task<User> GetCurrentUserAsync() =>
            _context.Users.FirstOrDefaultAsync(u => u.Id == GetCurrentUserId());
    }

}
