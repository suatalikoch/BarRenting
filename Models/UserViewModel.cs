using System.ComponentModel.DataAnnotations;

namespace BarRating.Models
{
    public class UserViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only alphabetic characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only alphabetic characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string PasswordConfirm { get; set; }
    }
}
