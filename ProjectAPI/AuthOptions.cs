using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProjectAPI
{
	/// <summary>
    /// This class provides values of authorization options for JWT token generation.
    /// </summary>
	public class AuthOptions
	{
		/// <summary>
        /// The issuer of the token.
        /// </summary>
		public const string ISSUER = "localhost";

		/// <summary>
		/// The audience of the token.
		/// </summary>
		public const string AUDIENCE = "localhost";

		/// <summary>
		/// The secret key.
		/// </summary>
		const string KEY = "mysupersecret_secretkey!123";

		/// <summary>
        /// The lifetime of the token in minutes.
        /// </summary>
		public const int LIFETIME = 180;

		/// <summary>
        /// Generates a security key based on <see cref="KEY"/>.
        /// </summary>
        /// <returns><see cref="SymmetricSecurityKey"/></returns>
		public static SymmetricSecurityKey GetSymmetricSecurityKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
		}
	}
}