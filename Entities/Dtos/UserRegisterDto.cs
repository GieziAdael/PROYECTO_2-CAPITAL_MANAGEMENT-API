namespace API_CAPITAL_MANAGEMENT.Entities.Dtos
{
    /// <summary>
    /// Data Transfer Object for user registration.
    /// </summary>
    public class UserRegisterDto
    {
        /// <summary>
        /// Property representing the unique identifier of the user.
        /// </summary>
        public string? ID { get; set; }
        /// <summary>
        /// Property representing the normalized email of the user.
        /// </summary>
        public string? EmailNormalized { get; set; }
        /// <summary>
        /// Property representing the password hash of the user.
        /// </summary>
        public string? PasswordHash { get; set; }
    }
}
