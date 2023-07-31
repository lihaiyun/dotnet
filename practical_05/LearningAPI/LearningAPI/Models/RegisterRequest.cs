using System.ComponentModel.DataAnnotations;

namespace LearningAPI.Models
{
    public class RegisterRequest
    {
        [Required, MinLength(3), MaxLength(50)]
        // Regular expression to enforce name format
        // letters, spaces, apostrophes, hyphens, commas and periods
        [RegularExpression(@"^[a-zA-Z '-,.]+$", ErrorMessage = "Invalid name")]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(50)]
        // Regular expression to enforce password complexity
        // at least one letter, at least one number and 8-50 characters
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*[0-9]).{8,50}$", ErrorMessage = "Invalid password")]
        public string Password { get; set; } = string.Empty;
    }
}
