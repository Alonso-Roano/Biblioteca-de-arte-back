namespace Babel.Models.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Alias { get; set; }
        public string Password { get; set; }
    }
}
