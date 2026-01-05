using System.ComponentModel.DataAnnotations;

namespace TaskManagement.ViewModels
{
    public class RegisterViewModel
    {
        public required string Name { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public required string ConfirmPassword { get; set; }
    }
}
