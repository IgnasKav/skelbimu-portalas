using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        //[RegularExpression("(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{4, 20}$", ErrorMessage = "Password must be complex")]
        
        public string Password { get; set; }

        [Required]
        public string Username { get; set; }
    }
}