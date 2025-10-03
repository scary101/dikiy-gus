using gus_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        public Task<User> GetCurrentUserAsync() =>
            _context.Users.FirstOrDefaultAsync(u => u.Id == GetCurrentUserId());
    }

}
