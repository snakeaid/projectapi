using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using ProjectAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace ProjectAPI.Controllers
{
	/// <summary>
	/// API controller class which controls authentication and derives from <see cref="ControllerBase"/>.
	/// </summary>
	[ApiController]
	[Route("api/")]
	public class AccountController : ControllerBase
	{
		/// <summary>
        /// List of all existing users.
        /// </summary>
		private List<User> users = new List<User>() { new User { Login = "manager", Password = "12345", Role = "Manager" } };

		/// <summary>
		/// An instance of <see cref="ILogger"/> which is used for logging.
		/// </summary>
		private readonly ILogger _logger;

		/// <summary>
		/// Constructs an instance of <see cref="AccountController"/> using the specified logger.
		/// </summary>
		/// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>
		/// for <see cref="AccountController"/>.</param>
		public AccountController(ILogger<AccountController> logger)
        {
			_logger = logger;
        }

		/// <summary>
		/// Handles the HTTP POST request to get a JWT token for authorization.
		/// </summary>
		/// <param name="username">The name of the user.</param>
		/// <param name="password">The password of the user.</param>
		/// <returns><see cref="IActionResult"/></returns>
		[HttpPost("authorize")]
		[AllowAnonymous]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		public IActionResult Token(string username, string password)
        {
			_logger.LogInformation($"Generating token for {username}");
			var identity = GetIdentity(username, password);
			if (identity==null)
            {
				_logger.LogWarning($"Invalid username or password.");
				return BadRequest(new { errorMessage = "Invalid username or password." });
			}

			var now = DateTime.UtcNow;
			var jwt = new JwtSecurityToken(
					issuer: AuthOptions.ISSUER,
					audience: AuthOptions.AUDIENCE,
					notBefore: now,
					claims: identity.Claims,
					expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
					signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			var response = new
			{
				access_token = encodedJwt,
				username = identity.Name
			};

			_logger.LogInformation($"Generated token for {username} successfully.");

			return Ok(response);
		}

		/// <summary>
        /// Verifies the identity and returns its claims.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns><see cref="ClaimsIdentity"/></returns>
		private ClaimsIdentity GetIdentity(string username, string password)
        {
			User User = users.FirstOrDefault(x => x.Login == username && x.Password == password);
			if(User!=null)
            {
				var claims = new List<Claim>
				{
					new Claim(ClaimsIdentity.DefaultNameClaimType,User.Login),
					new Claim(ClaimsIdentity.DefaultRoleClaimType, User.Role)
				};
				ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
				return claimsIdentity;
            }

			return null;
        }
	}
}