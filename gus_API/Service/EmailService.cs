using gus_API.Models;
using System.Net;
using System.Net.Mail;

namespace gus_API.Service
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly AppDbContext _context;

        public EmailService(IConfiguration configuration, AppDbContext context)
        {
            var emailSettings = configuration.GetSection("EmailSettings");
            _smtpServer = emailSettings["SmtpServer"];
            _smtpPort = int.Parse(emailSettings["SmtpPort"]);
            _smtpUser = emailSettings["SmtpUser"];
            _smtpPass = emailSettings["SmtpPass"];
            _context = context;
        }

        public async Task SendEmailAsync(string to, string subject, string code)
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                client.EnableSsl = true;

                string body = $"Ваш код подтверждения: <b>{code}</b>";

                var message = new MailMessage(_smtpUser, to, subject, body)
                {
                    IsBodyHtml = true
                };

                await client.SendMailAsync(message);
            }
        }

        public string GenerateLoginCode(User user)
        {
            var code = new Random().Next(100000, 1000000).ToString();

            var verCode = new ConfirmationCode
            {
                UserId = user.Id,
                Code = code,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now + TimeSpan.FromMinutes(10)
            };

            _context.ConfirmationCodes.Add(verCode);
            _context.SaveChanges();
            return code;
        }
    }
}
