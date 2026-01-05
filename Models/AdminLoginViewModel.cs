using System.ComponentModel.DataAnnotations;

namespace TaskManagement.ViewModels
{
    public class AdminLoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
