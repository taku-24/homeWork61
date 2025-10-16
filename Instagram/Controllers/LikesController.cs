using Instagram.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Instagram.Controllers;


[Authorize]
public class LikesController : Controller
{
    private readonly InstagramContext _context;
    private readonly UserManager<User> _userManager;
    public LikesController(InstagramContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int postId)
    {
        var me = await _userManager.GetUserAsync(User);
        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
            return Json(new { success = false, message = "Пост не найден" });

        var like = await _context.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == me.Id);
        bool liked;

        if (like == null)
        {
            _context.Likes.Add(new Like { PostId = postId, UserId = me.Id });
            post.LikesCount++;
            liked = true;
        }
        else
        {
            _context.Likes.Remove(like);
            post.LikesCount = Math.Max(0, post.LikesCount - 1);
            liked = false;
        }

        await _context.SaveChangesAsync();
        return Json(new { success = true, liked, likesCount = post.LikesCount });
    }
}