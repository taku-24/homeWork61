using System.ComponentModel.DataAnnotations;

namespace Instagram.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Имя")]
        public string? Name { get; set; }

        [Display(Name = "Информация о пользователе")]
        public string? UserInfo { get; set; }

        [Phone]
        [Display(Name = "Телефон")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Пол")]
        public string? Gender { get; set; }

        [Required]
        [Display(Name = "Аватар")]
        public IFormFile Image { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }
    }
}