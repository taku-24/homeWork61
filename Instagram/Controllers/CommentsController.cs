using Instagram.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Instagram.Controllers;
[Authorize]
public class CommentsController : Controller
{
    private readonly InstagramContext _context;
    private readonly UserManager<User> _userManager;

    public CommentsController(InstagramContext context, UserManager<User> userManager)
    {
        _context = context; 
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int postId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return RedirectToAction("Index", "Home");

        var me = await _userManager.GetUserAsync(User);
        var post = await _context.Posts.FindAsync(postId);
        if (post == null) return NotFound();

        _context.Comments.Add(new Comment
        {
            PostId = postId,
            UserId = me.Id,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow
        });
        post.CommentsCount++;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(int postId)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return PartialView("_CommentsList", comments);
    }

}