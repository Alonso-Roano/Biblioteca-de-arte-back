namespace Babel.Models.DTOs.Auth
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string AspNetUserId { get; set; } 
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Alias { get; set; }
        public string UserName { get; set; }
    }
}
