using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ProjectAPI.Models;
using Microsoft.Extensions.Logging;

namespace ProjectAPI.Controllers
{
	public class AccountController : Controller
	{
		List<Person> people = new List<Person> { new Person { Login = "manager", Password = "12345", Role = "Manager" } };
		private readonly ILogger _logger;

		public AccountController(ILogger<AccountController> logger)
        {
			_logger = logger;
        }

		[HttpPost("/token")]
		public IActionResult Token(string username, string password)
        {
			_logger.LogInformation($"Generating token for {username}");
			var identity = GetIdentity(username, password);
			if (identity==null)
            {
				_logger.LogWarning($"Invalid username or password.");
				return BadRequest(new { errorText = "Invalid username or password." });
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

			return Json(response);
		}

		private ClaimsIdentity GetIdentity(string username, string password)
        {
			Person person = people.FirstOrDefault(x => x.Login == username && x.Password == password);
			if(person!=null)
            {
				var claims = new List<Claim>
				{
					new Claim(ClaimsIdentity.DefaultNameClaimType,person.Login),
					new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role)
				};
				ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
				return claimsIdentity;
            }

			return null;
        }
	}
}

