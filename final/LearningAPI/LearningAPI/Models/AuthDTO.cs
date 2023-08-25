namespace LearningAPI.Models
{
    public class AuthDTO
    {
        public UserDTO User { get; set; } = new UserDTO();

        public string? AccessToken { get; set; }
    }
}
