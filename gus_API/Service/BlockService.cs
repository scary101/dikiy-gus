using gus_API.Models;

namespace gus_API.Service
{
    public class BlockService
    {
        private readonly AppDbContext _context;

        public BlockService(AppDbContext context) {  _context = context; }

        public static void BanUserHightAttempt(User user)
        {
            UserBan ban = new UserBan();
            if (user != null)
            {
                user.IsActive = false;
                user.UpdatedAt = DateTime.Now;
                ban.User = user;
                ban.StartDate = DateTime.Now;
                ban.EndDate = DateTime.Now.AddMinutes(30);
                ban.Reason = "Аккаунт пользователя заблокирован по причине: \"Привышен лимит попыток входа в аккаунт\"";

            }
        }

    }
}
