using System.ComponentModel.DataAnnotations;

namespace Instagram.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required] public string ImagePath { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int LikesCount { get; set; } = 0;
        public int CommentsCount { get; set; } = 0;

        public int UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; }
    }
}