using Instagram.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Instagram.Controllers;

[Authorize]
public class LikesController : Controller
{
    private readonly InstagramContext _ctx;
    private readonly UserManager<User> _um;
    public LikesController(InstagramContext ctx, UserManager<User> um) { _ctx = ctx; _um = um; }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int postId)
    {
        var me = await _um.GetUserAsync(User);
        var post = await _ctx.Posts.FindAsync(postId);
        if (post == null) return NotFound();

        var like = await _ctx.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == me.Id);
        if (like == null)
        {
            _ctx.Likes.Add(new Like { PostId = postId, UserId = me.Id });
            post.LikesCount++;
        }
        else
        {
            _ctx.Likes.Remove(like);
            post.LikesCount = Math.Max(0, post.LikesCount - 1);
        }
        await _ctx.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
}

