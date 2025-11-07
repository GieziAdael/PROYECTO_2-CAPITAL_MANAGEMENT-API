namespace API_CAPITAL_MANAGEMENT.Entities.Dtos
{
    /// <summary>
    /// Data Transfer Object for user login response.
    /// </summary>
    public class UserLoginResponseDto
    {
        /// <summary>
        /// Property representing the registered user.
        /// </summary>
        public UserRegisterDto? User { get; set; }
        /// <summary>
        /// Property representing the authentication token.
        /// </summary>
        public string? Token { get; set; }
        /// <summary>
        /// Property representing a message related to the login response.
        /// </summary>
        public string? Message { get; set; }
    }
}
