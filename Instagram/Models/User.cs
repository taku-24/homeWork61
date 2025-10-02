using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Instagram.Models
{
    public class User : IdentityUser<int>
    {

        [Required]
        public string ProfilePicturePath { get; set; } = "/images/default-avatar.png";

        public string? Name { get; set; }
        public string? UserInfo { get; set; }
        public string? Gender { get; set; }

        public int PostsCount { get; set; } = 0;
        public int FollowersCount { get; set; } = 0;
        public int FollowingCount { get; set; } = 0;
        
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Follow>? Followers { get; set; }
        public ICollection<Follow>? Following { get; set; }
    }
}