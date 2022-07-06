namespace ProjectAPI.Models
{
	/// <summary>
    /// Describes the user model which is used for authentication.  
    /// </summary>
	public class User
	{
		/// <summary>
        /// Gets and sets user's login.
        /// </summary>
		public string Login { get; set; }

		/// <summary>
		/// Gets and sets user's password.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Gets and sets user's role.
		/// </summary>
		public string Role { get; set; }
	}
}

