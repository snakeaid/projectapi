using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProjectAPI
{
	public class AuthOptions
	{
		public const string ISSUER = "localhost"; // издатель токена
		public const string AUDIENCE = "localhost"; // потребитель токена
		const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
		public const int LIFETIME = 180; // время жизни токена - 180 минут
		public static SymmetricSecurityKey GetSymmetricSecurityKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
		}
	}
}