using System.ComponentModel.DataAnnotations;

namespace skinet.identity.Dtos
{
    public class RegisterDto
    {
        // Restrictions due using of Authentication with DDD principles
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^((?=.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).*|(?=.{8,}$)(?=.*\\d)(?=.*[a-zA-Z])(?=.*[!\u0022#$%&'()*+,./:;<=>?@[\\]\\^_`{|}~-]).*)",
        ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase, 1 number, 1 non alphanumeric and at least 6 characters ")]
        public string Password { get; set; }
    }
}