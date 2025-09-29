using Microsoft.IdentityModel.Tokens;
using System.Text;

public static class Token
{
    public const string ISSUER = "Server";     // ← исправь на Server
    public const string AUDIENCE = "Client";   // ← исправь на Client
    const string KEY = "GusSuperSecretKey_ChangeThis_1234567890!!@#$%^&*"; // ← тот же ключ что в appsettings.json
    public const int LIFETIME = 60;

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY)); // ← UTF8 вместо ASCII
    }
}