using gus_API.Models;
using gus_API.Models.DTOs.AccountDTOs;
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

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                client.EnableSsl = true;

                var message = new MailMessage(_smtpUser, to, subject, body)
                {
                    IsBodyHtml = true
                };

                await client.SendMailAsync(message);
            }
        }
        public async Task SendConfirmCode(string email, string code)
        {
            var subject = "🔐 Код подтверждения входа";
            var body = $@"<!DOCTYPE html>
<html lang=""ru"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            margin: 0;
            padding: 20px;
            min-height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
        }}
        .container {{
            max-width: 500px;
            background: #ffffff;
            border-radius: 16px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);
            color: white;
            padding: 30px 20px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
            font-weight: 600;
        }}
        .content {{
            padding: 40px 30px;
            color: #333;
        }}
        .code-container {{
            background: #f8f9fa;
            border: 2px dashed #4CAF50;
            border-radius: 12px;
            padding: 25px;
            text-align: center;
            margin: 25px 0;
        }}
        .code {{
            font-family: 'Courier New', monospace;
            font-size: 32px;
            font-weight: bold;
            color: #2c5530;
            letter-spacing: 8px;
            background: #ffffff;
            padding: 15px;
            border-radius: 8px;
            display: inline-block;
            border: 1px solid #e0e0e0;
        }}
        .warning {{
            background: #fff3cd;
            border: 1px solid #ffeaa7;
            color: #856404;
            padding: 15px;
            border-radius: 8px;
            margin: 20px 0;
            font-size: 14px;
            text-align: center;
        }}
        .instructions {{
            color: #666;
            line-height: 1.6;
            margin-bottom: 20px;
        }}
        .footer {{
            background: #f8f9fa;
            padding: 20px;
            text-align: center;
            color: #6c757d;
            font-size: 12px;
            border-top: 1px solid #e9ecef;
        }}
        .icon {{
            font-size: 48px;
            margin-bottom: 15px;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""icon"">🔒</div>
            <h1>Подтверждение входа</h1>
        </div>
        
        <div class=""content"">
            <p class=""instructions"">
                Для завершения входа в ваш аккаунт используйте следующий код подтверждения:
            </p>
            
            <div class=""code-container"">
                <div class=""code"">{code}</div>
            </div>
            
            <div class=""warning"">
                ⚠️ <strong>Никому не сообщайте этот код!</strong><br>
                Код действителен в течение 10 минут.
            </div>
            
            <p style=""text-align: center; color: #666; margin-top: 25px;"">
                Если вы не запрашивали вход, проигнорируйте это письмо.
            </p>
        </div>
        
        <div class=""footer"">
            <p>С уважением,<br>Служба безопасности платформы</p>
            <p style=""margin-top: 10px; font-size: 11px; color: #999;"">
                Это автоматическое сообщение, пожалуйста, не отвечайте на него.
            </p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string email, string token)
        {
            var link = $"https://localhost:7070/reset-password?token={token}";
            var subject = "🔄 Сброс пароля";
            var body = $@"<!DOCTYPE html>
<html lang=""ru"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            margin: 0;
            padding: 20px;
            min-height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
        }}
        .container {{
            max-width: 550px;
            background: #ffffff;
            border-radius: 16px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #ff6b6b 0%, #ee5a52 100%);
            color: white;
            padding: 30px 20px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
            font-weight: 600;
        }}
        .content {{
            padding: 40px 30px;
            color: #333;
        }}
        .button-container {{
            text-align: center;
            margin: 30px 0;
        }}
        .reset-button {{
            display: inline-block;
            background: linear-gradient(135deg, #ff6b6b 0%, #ee5a52 100%);
            color: white;
            text-decoration: none;
            padding: 16px 40px;
            border-radius: 50px;
            font-size: 18px;
            font-weight: 600;
            box-shadow: 0 4px 15px rgba(255, 107, 107, 0.4);
            transition: all 0.3s ease;
        }}
        .reset-button:hover {{
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(255, 107, 107, 0.6);
        }}
        .link-backup {{
            display: block;
            text-align: center;
            margin-top: 15px;
            color: #666;
            font-size: 14px;
            word-break: break-all;
        }}
        .warning {{
            background: #fff3cd;
            border: 1px solid #ffeaa7;
            color: #856404;
            padding: 15px;
            border-radius: 8px;
            margin: 20px 0;
            font-size: 14px;
        }}
        .instructions {{
            color: #666;
            line-height: 1.6;
            margin-bottom: 20px;
        }}
        .footer {{
            background: #f8f9fa;
            padding: 20px;
            text-align: center;
            color: #6c757d;
            font-size: 12px;
            border-top: 1px solid #e9ecef;
        }}
        .icon {{
            font-size: 48px;
            margin-bottom: 15px;
        }}
        .urgency {{
            background: #ffebee;
            border-left: 4px solid #ff6b6b;
            padding: 15px;
            margin: 20px 0;
            border-radius: 0 8px 8px 0;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <div class=""icon"">🔄</div>
            <h1>Сброс пароля</h1>
        </div>
        
        <div class=""content"">
            <p class=""instructions"">
                Мы получили запрос на сброс пароля для вашего аккаунта. 
                Для создания нового пароля нажмите на кнопку ниже:
            </p>
            
            <div class=""button-container"">
                <a href=""{link}"" class=""reset-button"">Сбросить пароль</a>
                <a href=""{link}"" class=""link-backup"">{link}</a>
            </div>
            
            <div class=""urgency"">
                <strong>⚠️ Ссылка действительна 1 час</strong><br>
                По соображениям безопасности не пересылайте это письмо другим лицам.
            </div>
            
            <div class=""warning"">
                <strong>Важно:</strong> Если вы не запрашивали сброс пароля, 
                немедленно обратитесь в службу поддержки.
            </div>
        </div>
        
        <div class=""footer"">
            <p>С уважением,<br>Служба поддержки платформы</p>
            <p style=""margin-top: 10px; font-size: 11px; color: #999;"">
                Это автоматическое сообщение, пожалуйста, не отвечайте на него.
            </p>
        </div>
    </div>
</body>
</html>";

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
        public async Task SendEmailBanEntry(string email, bool active)
        {
            if (active)
            {
                var subject = "Уведомление от платформы";
                var body = @"<!DOCTYPE html>
<html lang=""ru"">
<head>
  <meta charset=""UTF-8"">
  <style>
    body { font-family: Arial, sans-serif; background-color: #f4f6f8; color: #333; }
    .container { max-width: 600px; margin: auto; background: #fff; padding: 20px; border-radius: 8px; }
    .header { background: #4CAF50; color: #fff; padding: 15px; text-align: center; border-radius: 8px 8px 0 0; }
    .content { margin: 20px 0; font-size: 16px; }
    .footer { font-size: 12px; color: #777; text-align: center; margin-top: 20px; }
  </style>
</head>
<body>
  <div class=""container"">
    <div class=""header"">
      <h2>Ваш аккаунт активирован 🎉</h2>
    </div>
    <div class=""content"">
      <p>Уважаемый пользователь,</p>
      <p>Ваш аккаунт был успешно <b>активирован</b> администратором платформы.</p>
      <p>Теперь вы можете:</p>
      <ul>
        <li>полностью использовать все функции платформы,</li>
        <li>совершать покупки и оформлять заказы,</li>
        <li>участвовать в акциях и получать бонусы.</li>
      </ul>
      <p>Благодарим за использование нашего сервиса и желаем приятного использования!</p>
    </div>
    <div class=""footer"">
      <p>С уважением,<br>Команда поддержки</p>
    </div>
  </div>
</body>
</html>";
                await SendEmailAsync(email, subject, body);
            }
            else
            {
                var subject = "Уведомление от платформы";
                var body = @"<!DOCTYPE html>
<html lang=""ru"">
<head>
  <meta charset=""UTF-8"">
  <style>
    body { font-family: Arial, sans-serif; background-color: #f4f6f8; color: #333; }
    .container { max-width: 600px; margin: auto; background: #fff; padding: 20px; border-radius: 8px; }
    .header { background: #e53935; color: #fff; padding: 15px; text-align: center; border-radius: 8px 8px 0 0; }
    .content { margin: 20px 0; font-size: 16px; }
    .footer { font-size: 12px; color: #777; text-align: center; margin-top: 20px; }
  </style>
</head>
<body>
  <div class=""container"">
    <div class=""header"">
      <h2>Ваш аккаунт деактивирован ⚠️</h2>
    </div>
    <div class=""content"">
      <p>Уважаемый пользователь,</p>
      <p>К сожалению, ваш аккаунт был <b>деактивирован</b> администратором платформы.</p>
      <p>Возможные причины:</p>
      <ul>
        <li>нарушение правил использования платформы,</li>
        <li>подозрительная активность в аккаунте,</li>
        <li>по вашему запросу.</li>
      </ul>
      <p>Для выяснения причин и восстановления доступа обратитесь в службу поддержки.</p>
    </div>
    <div class=""footer"">
      <p>С уважением,<br>Команда поддержки</p>
    </div>
  </div>
</body>
</html>";
                await SendEmailAsync(email, subject, body);
            }
        }
        public async Task SendEpmlRegisterInfo(InfoRegEmailDto model)
        {
            var fullName = $"{model.LastName} {model.FirstName} {model.MiddleName}".Trim();

            var subject = "Регистрация нового сотрудника";
            var body = $@"<!DOCTYPE html>
<html lang=""ru"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body {{ 
            font-family: 'Arial', sans-serif; 
            background-color: #f4f6f8; 
            color: #333; 
            margin: 0;
            padding: 20px;
        }}
        .container {{ 
            max-width: 600px; 
            margin: auto; 
            background: #fff; 
            padding: 30px; 
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
        .header {{ 
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: #fff; 
            padding: 25px; 
            text-align: center; 
            border-radius: 10px 10px 0 0;
            margin: -30px -30px 30px -30px;
        }}
        .header h2 {{
            margin: 0;
            font-size: 24px;
            font-weight: 600;
        }}
        .content {{ 
            margin: 25px 0; 
            font-size: 16px;
            line-height: 1.6;
        }}
        .credentials {{
            background: #f8f9fa;
            border-left: 4px solid #667eea;
            padding: 20px;
            margin: 25px 0;
            border-radius: 0 8px 8px 0;
        }}
        .credentials h3 {{
            margin-top: 0;
            color: #495057;
            font-size: 18px;
        }}
        .credential-item {{
            margin: 12px 0;
            display: flex;
            align-items: center;
        }}
        .credential-label {{
            font-weight: 600;
            color: #495057;
            min-width: 120px;
        }}
        .credential-value {{
            background: #fff;
            padding: 8px 12px;
            border-radius: 6px;
            border: 1px solid #e9ecef;
            font-family: 'Courier New', monospace;
            flex-grow: 1;
        }}
        .warning {{
            background: #fff3cd;
            border: 1px solid #ffeaa7;
            color: #856404;
            padding: 15px;
            border-radius: 8px;
            margin: 20px 0;
            font-size: 14px;
        }}
        .footer {{ 
            font-size: 14px; 
            color: #6c757d; 
            text-align: center; 
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #e9ecef;
        }}
        .greeting {{
            font-size: 18px;
            font-weight: 600;
            color: #495057;
            margin-bottom: 20px;
        }}
        .instructions {{
            margin: 20px 0;
        }}
        .instructions ol {{
            margin: 15px 0;
            padding-left: 20px;
        }}
        .instructions li {{
            margin-bottom: 10px;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>👋 Добро пожаловать в команду!</h2>
        </div>
        
        <div class=""content"">
            <div class=""greeting"">
                Уважаемый(ая) {fullName},
            </div>
            
            <p>Мы рады приветствовать вас в нашей команде! Ваш аккаунт был успешно зарегистрирован в системе.</p>
            
            <div class=""credentials"">
                <h3>🔐 Данные для входа в систему:</h3>
                
                <div class=""credential-item"">
                    <span class=""credential-label"">Логин (Email):</span>
                    <span class=""credential-value"">{model.Email}</span>
                </div>
                
                <div class=""credential-item"">
                    <span class=""credential-label"">Пароль:</span>
                    <span class=""credential-value"">{model.Password}</span>
                </div>
            </div>
            
            <div class=""warning"">
                ⚠️ <strong>В целях безопасности</strong> рекомендуем сменить пароль при первом входе в систему.
            </div>
            
            <div class=""instructions"">
                <p><strong>Для начала работы:</strong></p>
                <ol>
                    <li>Перейдите на страницу входа в систему</li>
                    <li>Используйте указанные выше данные для авторизации</li>
                    <li>Смените пароль в личном кабинете</li>
                    <li>Ознакомьтесь с руководством пользователя</li>
                </ol>
            </div>
            
            <p>Если у вас возникнут вопросы или потребуется помощь, обращайтесь в техническую поддержку.</p>
        </div>
        
        <div class=""footer"">
            <p>С уважением,<br>
            <strong>Отдел кадров и IT-поддержка</strong><br>
            {DateTime.Now.Year}</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(model.Email, subject, body);
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
