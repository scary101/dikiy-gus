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
        public async Task SendPasswordResetEmailAsync(string email, string token)
        {
            var link = $"https://localhost:7070/{token}";
            var subject = "Сброс пароля";
            var body = $"<p>Для сброса пароля перейдите по <a href='{link}'>ссылке</a>.</p>";


            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEmailEpEntry(string email, bool active)
        {
            if(active)
            {
                var subject = "Уведомление для ИП";
                var body = $"<!DOCTYPE html>\r\n<html lang=\"ru\">\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <style>\r\n    body {{ font-family: Arial, sans-serif; background-color: #f4f6f8; color: #333; }}\r\n    .container {{ max-width: 600px; margin: auto; background: #fff; padding: 20px; border-radius: 8px; }}\r\n    .header {{ background: #4CAF50; color: #fff; padding: 15px; text-align: center; border-radius: 8px 8px 0 0; }}\r\n    .content {{ margin: 20px 0; font-size: 16px; }}\r\n    .footer {{ font-size: 12px; color: #777; text-align: center; margin-top: 20px; }}\r\n  </style>\r\n</head>\r\n<body>\r\n  <div class=\"container\">\r\n    <div class=\"header\">\r\n      <h2>Аккаунт ИП активирован 🎉</h2>\r\n    </div>\r\n    <div class=\"content\">\r\n      <p>Уважаемый предприниматель,</p>\r\n      <p>Ваш аккаунт был успешно <b>активирован</b> администратором платформы.</p>\r\n      <p>Теперь вы можете:</p>\r\n      <ul>\r\n        <li>размещать товары в магазине,</li>\r\n        <li>оформлять поставки,</li>\r\n        <li>получать выплаты на ваш расчетный счёт.</li>\r\n      </ul>\r\n      <p>Благодарим за доверие к нашей платформе и желаем успехов в бизнесе!</p>\r\n    </div>\r\n    <div class=\"footer\">\r\n      <p>С уважением,<br>Команда поддержки</p>\r\n    </div>\r\n  </div>\r\n</body>\r\n</html>";
                await SendEmailAsync(email, subject, body);
            }
            else
            {
                var subject = "Уведомление для ИП";
                var body = $"<!DOCTYPE html>\r\n<html lang=\"ru\">\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <style>\r\n    body {{ font-family: Arial, sans-serif; background-color: #f4f6f8; color: #333; }}\r\n    .container {{ max-width: 600px; margin: auto; background: #fff; padding: 20px; border-radius: 8px; }}\r\n    .header {{ background: #e53935; color: #fff; padding: 15px; text-align: center; border-radius: 8px 8px 0 0; }}\r\n    .content {{ margin: 20px 0; font-size: 16px; }}\r\n    .footer {{ font-size: 12px; color: #777; text-align: center; margin-top: 20px; }}\r\n  </style>\r\n</head>\r\n<body>\r\n  <div class=\"container\">\r\n    <div class=\"header\">\r\n      <h2>Аккаунт ИП деактивирован ⚠️</h2>\r\n    </div>\r\n    <div class=\"content\">\r\n      <p>Уважаемый предприниматель,</p>\r\n      <p>К сожалению, ваш аккаунт был <b>деактивирован</b> администратором платформы.</p>\r\n      <p>Возможные причины:</p>\r\n      <ul>\r\n        <li>несоблюдение правил площадки,</li>\r\n        <li>предоставление некорректных данных,</li>\r\n        <li>по запросу самого пользователя.</li>\r\n      </ul>\r\n      <p>Для получения дополнительной информации вы можете связаться с нашей службой поддержки.</p>\r\n    </div>\r\n    <div class=\"footer\">\r\n      <p>С уважением,<br>Команда поддержки</p>\r\n    </div>\r\n  </div>\r\n</body>\r\n</html>\r\n";
                await SendEmailAsync(email, subject, body);
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
