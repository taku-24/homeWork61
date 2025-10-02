using System.ComponentModel.DataAnnotations;

namespace Instagram.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Логин или Email")]
        public string Login { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}