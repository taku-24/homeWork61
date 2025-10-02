using System.ComponentModel.DataAnnotations;

namespace Instagram.ViewModels
{
    public class EditProfileViewModel
    {
        [Display(Name = "Имя")]
        public string? Name { get; set; }

        [Display(Name = "Информация о пользователе")]
        public string? UserInfo { get; set; }

        [Display(Name = "Пол")]
        public string? Gender { get; set; }

        [Display(Name = "Аватар")]
        public IFormFile? ProfilePicture { get; set; }
    }
}
