using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace gus_API.Models
{
    public class Token
    {
        public const string ISSUER = "Gus"; 
        public const string AUDIENCE = "Gusina"; 
        const string KEY = "gussssssssss123123";   
        public const int LIFETIME = 15; 
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
