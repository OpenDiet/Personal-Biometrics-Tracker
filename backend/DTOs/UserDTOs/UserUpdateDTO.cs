namespace PersonalBiometricsTracker.Dtos
{
    public class UserUpdateDto : UserRegistrationDto
    {
        public new string? Username { get; set; }

        public new string? Email { get; set; }

        public new string? Password { get; set; }
    }
}