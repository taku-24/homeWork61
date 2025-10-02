using Instagram.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Instagram.Controllers;
[Authorize]
public class CommentsController : Controller
{
    private readonly InstagramContext _ctx;
    private readonly UserManager<User> _um;
    public CommentsController(InstagramContext ctx, UserManager<User> um) { _ctx = ctx; _um = um; }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int postId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return RedirectToAction("Index", "Home");

        var me = await _um.GetUserAsync(User);
        var post = await _ctx.Posts.FindAsync(postId);
        if (post == null) return NotFound();

        _ctx.Comments.Add(new Comment
        {
            PostId = postId,
            UserId = me.Id,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow
        });
        post.CommentsCount++;
        await _ctx.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(int postId)
    {
        var comments = await _ctx.Comments
            .Include(c => c.User)
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return PartialView("_CommentsList", comments);
    }

}
