using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Instagram.Models
{
    public class InstagramContext : IdentityDbContext<User, IdentityRole<int>, int>
    {

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Follow> Follows { get; set; }

        public InstagramContext(DbContextOptions<InstagramContext> options) : base(options) { }
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Following)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follow>()
                .HasIndex(f => new { f.FollowerId, f.FollowingId })
                .IsUnique();
            
            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.PostId, l.UserId })
                .IsUnique();
            
            
            modelBuilder.Entity<Follow>().HasIndex(f => new { f.FollowerId, f.FollowingId }).IsUnique();
            modelBuilder.Entity<Like>().HasIndex(l => new { l.PostId, l.UserId }).IsUnique();

            
            
            modelBuilder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int> { Id = 1, Name = "admin", NormalizedName = "ADMIN" },
                new IdentityRole<int> { Id = 2, Name = "user", NormalizedName = "USER" }
            );
        }
    }
}