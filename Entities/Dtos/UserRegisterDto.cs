namespace API_CAPITAL_MANAGEMENT.Entities.Dtos
{
    public class UserRegisterDto
    {
        public string? ID { get; set; }
        public string? EmailNormalized { get; set; }
        public string? PasswordHash { get; set; }
    }
}
